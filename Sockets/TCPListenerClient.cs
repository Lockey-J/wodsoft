using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Net.Security;
using System.Threading;

namespace TcpNet.Net.Sockets
{
	public class TCPListenerClient : SocketBase
	{
		internal TCPListenerClient(TCPListener listener, Socket socket) : base(socket, listener.Handler)
		{
			//创建Socket网络流
			Stream = new NetworkStream(socket);
			if (listener.IsUseAuthenticate)
			{
				NegotiateStream negotiate = new NegotiateStream(Stream);
				negotiate.AuthenticateAsServer();
				while (!negotiate.IsMutuallyAuthenticated)
				{
					Thread.Sleep(10);
				}
			}
			//设置服务器
			this.Listener = listener;

			//开始异步接收数据
			SocketAsyncState state = new SocketAsyncState();
			Handler.BeginReceive(Stream, EndReceive, state);
		}

		private TCPListener privateListener;
		public TCPListener Listener
		{
			get
			{
				return privateListener;
			}
			private set
			{
				privateListener = value;
			}
		}


	}
}
