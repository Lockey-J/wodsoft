using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Wodsoft.Net.Sockets;
//Imports System.Net.Sockets
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace Wodsoft.Net.Communication
{
	public class Client : CommunicationBase
	{
		// Fields
		private byte[] _ConnectHead;
		private bool _Connecting;

		// Events
		public event EventHandler<CommunicationConnectEventArgs> ConnectCompleted;

		// Methods
		public Client() : base(new TCPClient())
		{
			TCPClient client = (TCPClient)base.Socket;
			client.ConnectCompleted += client_ConnectCompleted;
			client.ReceiveCompleted += client_ReceiveCompleted;
			client.DisconnectCompleted += client_DisconnectCompleted;
		}

		private void client_ConnectCompleted(object sender, SocketEventArgs e)
		{
			if (e.Socket.IsConnected)
			{
				List<byte> data = new List<byte>() {0};
				if (_ConnectHead == null)
				{
					data.Add(0);
					data.Add(0);
				}
				else
				{
					data.AddRange(BitConverter.GetBytes((ushort)_ConnectHead.Length));
					data.AddRange(_ConnectHead);
				}
				if (Credential != null)
				{
					if (Credential.Username == null)
					{
						data.Add(0);
						data.Add(0);
					}
					else
					{
						data.AddRange(BitConverter.GetBytes((ushort)Credential.Username.Length));
						data.AddRange(Credential.Username);
					}
					if (Credential.Password != null)
					{
						data.AddRange(BitConverter.GetBytes((ushort)Credential.Password.Length));
						data.AddRange(Credential.Password);
					}
				}
				e.Socket.SendAsync(data.ToArray());
			}
			else
			{
				_Connecting = false;
				CommunicationConnectEventArgs eventArgs = new CommunicationConnectEventArgs(this, false, null);
				if (ConnectCompleted != null)
					ConnectCompleted(this, eventArgs);
			}
		}

		private void client_DisconnectCompleted(object sender, SocketEventArgs e)
		{
			if (_Connecting)
			{
				_Connecting = false;
			}
		}

		private void client_ReceiveCompleted(object sender, SocketEventArgs e)
		{
			if (((e.Data[0] == 0) && (e.DataLength >= 2)) && _Connecting)
			{
				bool success = e.Data[1] == 1;
				byte[] head = null;
				if (!success && (e.DataLength > 2))
				{
					head = e.Data.Skip(2).ToArray();
				}
				_Connecting = false;
				CommunicationConnectEventArgs eventArgs = new CommunicationConnectEventArgs(this, success, head);
				if (ConnectCompleted != null)
					ConnectCompleted(this, eventArgs);
			}
		}

		public void Connect(IPEndPoint endPoint, byte[] head)
		{
			if (_Connecting)
			{
				throw new InvalidOperationException("正在连接服务器。");
			}
			if (IsConnected)
			{
				throw new InvalidOperationException("已连接服务器。");
			}
			if ((head != null) && (head.Length > 48000))
			{
				throw new IndexOutOfRangeException("头信息长度不能大于48000。");
			}
			lock (this)
			{
				_Connecting = true;
				_ConnectHead = head;
				((TCPClient)base.Socket).ConnectAsync(endPoint);
			}
		}

		// Properties
		public Credential Credential {get; set;}
	}
}
