using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wodsoft.Net.Sockets;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace Wodsoft.Net.Communication
{
	/// <summary>
	/// 服务器端
	/// </summary>
	public class Server
	{
        // Fields
        private TCPListener Listener;

        // Events
        public event EventHandler<CommunicationAcceptEventArgs> AcceptCompleted;

		public event EventHandler<CommunicationAcceptEventArgs> PreviewAccept;

		// Methods
		public Server() : this(0)
		{
		}

		public Server(ushort port)
		{
			Listener = new TCPListener();
			Listener.AcceptCompleted += Listener_AcceptCompleted;
			Listener.Port = port;
		}

		private void AuthenticateTimeOut(object state, bool timedOut)
		{
			if (timedOut && ((ISocket)state).IsConnected)
			{
				((ISocket)state).Disconnect();
			}
			((ISocket)state)["timeout"] = null;
		}

		private void Listener_AcceptCompleted(object sender, SocketEventArgs e)
		{
			AutoResetEvent autoReset = new AutoResetEvent(false);
			e.Socket["timeout"] = autoReset;
			e.Socket.ReceiveCompleted += Socket_ReceiveCompleted;
			ServerClient client = new ServerClient(e.Socket);
			e.Socket["client"] = client;
			ThreadPool.RegisterWaitForSingleObject(autoReset, new WaitOrTimerCallback(AuthenticateTimeOut), e.Socket, 0x1388, true);
		}

		private void Socket_ReceiveCompleted(object sender, SocketEventArgs e)
		{
			if (e.Data[0] != 0)
			{
				e.Socket.Disconnect();
			}
			else
			{
				byte[] data = null;
				e.Socket.ReceiveCompleted -= Socket_ReceiveCompleted;
				ushort headLength = BitConverter.ToUInt16(e.Data.Skip(1).Take(2).ToArray(), 0);
				byte[] head = null;
				if (headLength > 0)
				{
					head = e.Data.Skip(3).Take(headLength).ToArray();
				}
				byte[] username = null;
				byte[] password = null;
				if (e.DataLength > (3 + headLength))
				{
					ushort usernameLength = BitConverter.ToUInt16(e.Data.Skip((3 + headLength)).Take(2).ToArray(), 0);
					if ((usernameLength != 0) && (e.DataLength >= ((5 + headLength) + usernameLength)))
					{
						username = e.Data.Skip((5 + headLength)).Take(usernameLength).ToArray();
						if ((usernameLength != 0) && (e.DataLength > ((5 + headLength) + usernameLength)))
						{
							ushort passwordLength = BitConverter.ToUInt16(e.Data.Skip(((5 + headLength) + usernameLength)).Take(2).ToArray(), 0);
							if ((passwordLength != 0) && (e.DataLength == (((7 + headLength) + usernameLength) + passwordLength)))
							{
								password = e.Data.Skip(((7 + headLength) + usernameLength)).ToArray();
							}
						}
					}
				}
				ServerClient client = (ServerClient)e.Socket["client"];
				((AutoResetEvent)e.Socket["timeout"]).Set();
				if ((username != null) || (password != null))
				{
					client.Credential = new Credential(username, password);
				}
				CommunicationAcceptEventArgs eventArgs = new CommunicationAcceptEventArgs(client, head, client.Credential);
				if (PreviewAccept != null)
					PreviewAccept(this, eventArgs);
				if (eventArgs.Handled && (eventArgs.FailedData != null))
				{
					data = new byte[(2 + eventArgs.FailedData.Length)];
				}
				else
				{
					data = new byte[2];
				}
				if (eventArgs.Handled)
				{
					data[1] = 0;
				}
				else
				{
					data[1] = 1;
				}
				if (eventArgs.Handled && (eventArgs.FailedData != null))
				{
					eventArgs.FailedData.CopyTo(data, 2);
				}
				e.Socket.Send(data);
				if (eventArgs.Handled)
				{
					e.Socket.Disconnect();
					client.Dispose();
				}
				else
				{
					client.IsAuthenticated = true;
					if (AcceptCompleted != null)
						AcceptCompleted(this, eventArgs);
				}
			}
		}

		public void Start()
		{
			Listener.Start();
		}

		public void Stop()
		{
			Listener.Stop();
		}

		// Properties
		public bool IsStarted
		{
			get
			{
				return Listener.IsStarted;
			}
		}

		public int Port
		{
			get
			{
				return Listener.Port;
			}
		}
	}
}
