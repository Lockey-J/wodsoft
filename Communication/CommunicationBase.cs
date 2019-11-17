using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Communication
{
	public class CommunicationBase : ICommunication, IDisposable
	{
		private ISocket privateSocket;
		protected ISocket Socket
		{
			get
			{
				return privateSocket;
			}
			private set
			{
				privateSocket = value;
			}
		}

		public CommunicationBase(ISocket socket)
		{
			if (socket == null)
			{
				throw new ArgumentNullException("socket");
			}

			_Disposed = false;

			SendCache = new Dictionary<byte[], CommunicationSendEventArgs>();
			SendMark = new Dictionary<CommunicationSendEventArgs, int>();
			SendWait = new Dictionary<Guid, CommunicationSendEventArgs>();

			ReceiveCache = new Dictionary<Guid, CommunicationReceiveEventArgs>();

			this.Socket = socket;
			socket.ReceiveCompleted += socket_ReceiveCompleted;
			socket.SendCompleted += socket_SendCompleted;
			socket.DisconnectCompleted += socket_DisconnectCompleted;
		}

		#region 断开连接

		public void Disconnect()
		{
			if (!IsConnected)
			{
				throw new InvalidOperationException("连接已断开。");
			}
			Socket.DisconnectAsync();
		}

		private void socket_DisconnectCompleted(object sender, SocketEventArgs e)
		{
			OnDisconnect();
		}

		private void OnDisconnect()
		{
			Socket.DisconnectCompleted -= socket_DisconnectCompleted;
			Socket.ReceiveCompleted -= socket_ReceiveCompleted;
			Socket.SendCompleted -= socket_SendCompleted;

			if (DisconnectCompleted != null)
				DisconnectCompleted(this, new CommunicationDisconnectEventArgs(this, null));
		}

		#endregion

		#region 接收数据

		private Dictionary<Guid, CommunicationReceiveEventArgs> ReceiveCache;

		private void socket_ReceiveCompleted(object sender, SocketEventArgs e)
		{
			byte[] data = null;
			Guid id = new Guid();
			CommunicationReceiveEventArgs receiveEventArgs = null;
			switch (e.Data[0])
			{
				case 3:
					if (e.DataLength < 21)
					{
						return;
					}
					id = new Guid(e.Data.Skip(1).Take(16).ToArray());
					int dataLength = BitConverter.ToInt32(e.Data.Skip(17).Take(4).ToArray(), 0);
					byte[] head = null;
					if (e.DataLength > 21)
					{
						head = e.Data.Skip(21).ToArray();
					}
					receiveEventArgs = new CommunicationReceiveEventArgs(this, id, dataLength, head);
                    PreviewReceive?.Invoke(this, receiveEventArgs);
                    data = new byte[18];
					data[0] = 4;
                    int i = 1;
                    do
                    {
                        data[i] = e.Data[i];
                        i++;
                    }
                    while (i <= 16);
                    if (!receiveEventArgs.Handled)
					{
						receiveEventArgs.Data = new byte[dataLength];
						ReceiveCache.Add(id, receiveEventArgs);
						data[17] = 1;
					}
					else
					{
						data[17] = 0;
					}
					Socket.SendAsync(data);
					break;
				case 4:
					if (e.DataLength != 18)
					{
						return;
					}
					id = new Guid(e.Data.Skip(1).Take(16).ToArray());
					if (!SendWait.ContainsKey(id))
					{
						return;
					}
					CommunicationSendEventArgs sendEventArgs = SendWait[id];
					SendWait.Remove(id);
					sendEventArgs.Success = e.Data[17] == 1;
					if (!sendEventArgs.Success)
					{
						if (SendCompleted != null)
							SendCompleted(this, sendEventArgs);
						return;
					}
					data = GetSendBlock(sendEventArgs, 0);
					SendCache.Add(data, sendEventArgs);
					SendMark.Add(sendEventArgs, data.Length - 21);
					Socket.SendAsync(data);
					break;
				case 5:
					if (e.DataLength < 22)
					{
						return;
					}
					id = new Guid(e.Data.Skip(1).Take(16).ToArray());
					if (!ReceiveCache.ContainsKey(id))
					{
						return;
					}
					receiveEventArgs = ReceiveCache[id];
					int offset = BitConverter.ToInt32(e.Data.Skip(17).Take(4).ToArray(), 0);
                    for (int j = 0; j < e.DataLength - 21; j++)
                    {
                        receiveEventArgs.Data[offset + j] = e.Data[21 + j];
                    }
                    if (offset + e.DataLength - 21 == receiveEventArgs.DataLength)
					{
						ReceiveCache.Remove(id);
                        ReceiveCompleted?.Invoke(this, receiveEventArgs);
                    }
					break;
			}
		}

		#endregion

		#region 发送数据

		private Dictionary<Guid, CommunicationSendEventArgs> SendWait;
		private Dictionary<byte[], CommunicationSendEventArgs> SendCache;
		private Dictionary<CommunicationSendEventArgs, int> SendMark;

		public void Send(byte[] data, byte[] head)
		{
			if (_Disposed)
			{
				throw new InvalidOperationException("已释放资源。");
			}
			if (!IsConnected)
			{
				throw new InvalidOperationException("连接已断开。");
			}
			if (head != null && head.Length > 48000)
			{
				throw new IndexOutOfRangeException("头信息长度不能大于48000。");
			}
			if (head != null && head.Length == 0)
			{
				head = null;
			}

			CommunicationSendEventArgs eventArgs = new CommunicationSendEventArgs(this, data, head);

			byte[] sendData = null;
			if (head == null)
			{
				sendData = new byte[21];
			}
			else
			{
				sendData = new byte[(21 + head.Length)];
			}
			sendData[0] = 3;
			eventArgs.DataID.ToByteArray().CopyTo(sendData, 1);
			BitConverter.GetBytes(data.Length).CopyTo(sendData, 17);
			if (head != null)
			{
				head.CopyTo(sendData, 21);
			}
			SendWait.Add(eventArgs.DataID, eventArgs);
			Socket.SendAsync(sendData);
		}

		private void socket_SendCompleted(object sender, SocketEventArgs e)
		{
			if (!SendCache.ContainsKey(e.Data))
			{
				return;
			}
			CommunicationSendEventArgs eventArgs = SendCache[e.Data];
			SendCache.Remove(e.Data);
			byte[] data = null;
			if (e.Data[0] == 5)
			{
				int offset = SendMark[eventArgs];
				if (offset < eventArgs.Data.Length)
				{
					data = GetSendBlock(eventArgs, offset);
					SendCache.Add(data, eventArgs);
					SendMark[eventArgs] = offset + data.Length - 21;
					Socket.SendAsync(data);
				}
				else
				{
					SendMark.Remove(eventArgs);
					if (SendCompleted != null)
						SendCompleted(this, eventArgs);
				}
			}
		}

		private byte[] GetSendBlock(CommunicationSendEventArgs eventArgs, int offset)
		{
			byte[] data = null;
			if (offset + 32000 < eventArgs.Data.Length)
			{
				data = new byte[32021];
				data[0] = 5;
				eventArgs.DataID.ToByteArray().CopyTo(data, 1);
				BitConverter.GetBytes(offset).CopyTo(data, 17);
				for (int i = 0; i <= 31999; i++)
				{
					data[21 + i] = eventArgs.Data[offset + i];
				}
				return data;
			}
			else
			{
				data = new byte[(21 + eventArgs.Data.Length - offset)];
				data[0] = 5;
				eventArgs.DataID.ToByteArray().CopyTo(data, 1);
				for (int i = 0; i < eventArgs.Data.Length - offset; i++)
				{
					data[21 + i] = eventArgs.Data[offset + i];
				}
				return data;
			}
		}

		#endregion

		public virtual bool IsConnected
		{
			get
			{
				return Socket.IsConnected && !_Disposed;
			}
		}

		public event EventHandler<CommunicationReceiveEventArgs> PreviewReceive;

		public event EventHandler<CommunicationReceiveEventArgs> ReceiveCompleted;

		public event EventHandler<CommunicationSendEventArgs> SendCompleted;

		public event EventHandler<CommunicationDisconnectEventArgs> DisconnectCompleted;

		public object this[string key]
		{
			get
			{
				if (_Disposed)
				{
					throw new InvalidOperationException("已释放资源。");
				}
				return Socket[key];
			}
			set
			{
				if (_Disposed)
				{
					throw new InvalidOperationException("已释放资源。");
				}
				Socket[key] = value;
			}
		}

		public System.Net.IPEndPoint EndPoint
		{
			get
			{
				if (_Disposed)
				{
					throw new InvalidOperationException("已释放资源。");
				}
				return Socket.RemoteEndPoint;
			}
		}

		private bool _Disposed;
		public void Dispose()
		{
			if (_Disposed)
			{
				throw new InvalidOperationException("已释放资源。");
			}
			if (IsConnected)
			{
				OnDisconnect();
			}
			SendCache.Clear();
			SendCache = null;
			SendMark.Clear();
			SendMark = null;
			SendWait.Clear();
			SendWait = null;
			ReceiveCache.Clear();
			ReceiveCache = null;
		}
	}
}
