using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Wodsoft.Net.Sockets
{
	/// <summary>
	/// UDP服务
	/// </summary>
	public class UDPService : IUDPService
	{
		private Socket Socket;

		/// <summary>
		/// 实例化UDP服务。
		/// </summary>
		public UDPService()
		{
			ReceiveBufferSize = 536;
			SendBufferSize = 536;
			IsStarted = false;
		}

		/// <summary>
		/// 服务启动中
		/// </summary>
		private bool privateIsStarted;
		public bool IsStarted
		{
			get
			{
				return privateIsStarted;
			}
			private set
			{
				privateIsStarted = value;
			}
		}

		/// <summary>
		/// 接受缓存大小。
		/// </summary>
		public int ReceiveBufferSize {get; set;}
		/// <summary>
		/// 发送缓存大小。
		/// </summary>
		public int SendBufferSize {get; set;}
//INSTANT VB NOTE: The field port was renamed since Visual Basic does not allow fields to have the same name as other class members:
		private int port_Renamed;
		/// <summary>
		/// 监听端口。
		/// </summary>
		public int Port
		{
			get
			{
				return port_Renamed;
			}
			set
			{
				if (value < 0 || value > 65535)
				{
					throw new ArgumentOutOfRangeException(port_Renamed + "不是有效端口。");
				}
				port_Renamed = value;
			}
		}

		/// <summary>
		/// 接收完成时引发事件。
		/// </summary>
		public event EventHandler<UDPServiceEventArgs> ReceiveCompleted;
		/// <summary>
		/// 发送完成时引发事件。
		/// </summary>
		public event EventHandler<UDPServiceEventArgs> SendCompleted;

		/// <summary>
		/// 开始服务
		/// </summary>
		public void Start(bool allowBroadcast = false)
		{
			if (IsStarted)
			{
				throw new InvalidOperationException("已经开始服务。");
			}
			Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			if (allowBroadcast)
			{
				Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, 1);
			}
			Socket.Bind(new IPEndPoint(IPAddress.Any, Port));
			Socket.ReceiveBufferSize = ReceiveBufferSize;
			Socket.SendBufferSize = SendBufferSize;
			CreateReceive();
			IsStarted = true;
		}

		/// <summary>
		/// 停止服务
		/// </summary>
		public void Stop()
		{
			if (!IsStarted)
			{
				throw new InvalidOperationException("没有开始服务。");
			}
			Socket.Close();
			Socket = null;
			IsStarted = false;
		}

		private void CreateReceive()
		{
			UDPServiceAsyncState state = new UDPServiceAsyncState();
			state.Data = new byte[ReceiveBufferSize];
			EndPoint ip = new IPEndPoint(IPAddress.Any, 0);
			Socket.BeginReceiveFrom(state.Data, 0, ReceiveBufferSize, SocketFlags.None, ref ip, EndReceive, state);
		}

		private void EndReceive(IAsyncResult result)
		{
			UDPServiceAsyncState state = (UDPServiceAsyncState)result.AsyncState;
			int dataLength = 0;
			EndPoint clientip = new IPEndPoint(IPAddress.Any, 0);
			try
			{
				dataLength = Socket.EndReceiveFrom(result, ref clientip);
			}
			catch
			{
				dataLength = 0;
			}
			byte[] data = state.Data;

			if (dataLength == 0)
			{
				return;
			}

			EndPoint ip = new IPEndPoint(IPAddress.Any, 0);
			state.Data = new byte[ReceiveBufferSize];
			Socket.BeginReceiveFrom(state.Data, 0, ReceiveBufferSize, SocketFlags.None, ref ip, EndReceive, state);

			if (ReceiveCompleted != null)
				ReceiveCompleted(this, new UDPServiceEventArgs
				{
					EndPoint = (IPEndPoint)clientip,
					Data = data,
					DataLength = dataLength,
					Operation = SocketAsyncOperation.ReceiveFrom
				});
		}

		/// <summary>
		/// 发送数据
		/// </summary>
		/// <param name="remote">远程终结点。</param>
		/// <param name="data">要发送的数据。</param>
		public void Send(IPEndPoint remote, byte[] data)
		{
			if (!IsStarted)
			{
				throw new InvalidOperationException("没有开始服务。");
			}

			UDPServiceAsyncState state = new UDPServiceAsyncState();
			state.Data = data;
			state.IsAsync = false;
			state.EndPoint = remote;
			Socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, remote, EndSend, state).AsyncWaitHandle.WaitOne();
		}

		/// <summary>
		/// 发送数据
		/// </summary>
		/// <param name="remote">远程终结点。</param>
		/// <param name="data">要发送的数据。</param>
		public void SendAsync(IPEndPoint remote, byte[] data)
		{
			if (!IsStarted)
			{
				throw new InvalidOperationException("没有开始服务。");
			}
			UDPServiceAsyncState state = new UDPServiceAsyncState();
			state.Data = data;
			state.IsAsync = true;
			state.EndPoint = remote;
			Socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, remote, EndSend, state);
		}

		private void EndSend(IAsyncResult result)
		{
			UDPServiceAsyncState state = (UDPServiceAsyncState)result.AsyncState;
			Socket.EndSendTo(result);

			if (state.IsAsync)
			{
				if (SendCompleted != null)
					SendCompleted(this, new UDPServiceEventArgs
					{
						EndPoint = state.EndPoint,
						Data = state.Data,
						DataLength = state.Data.Length,
						Operation = SocketAsyncOperation.SendTo
					});
			}
		}

		/// <summary>
		/// 获取本地终结点地址。
		/// </summary>
		public IPEndPoint LocalEndPoint
		{
			get
			{
				if (IsStarted)
				{
					return (IPEndPoint)Socket.LocalEndPoint;
				}
				return null;
			}
		}
	}
}
