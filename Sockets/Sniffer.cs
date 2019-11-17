using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Wodsoft.Net.Sockets
{
	public class Sniffer
	{
		private Socket Socket;
		private byte[] Buffer;

		public Sniffer()
		{
			Buffer = new byte[ushort.MaxValue];
		}

		public void Start(IPAddress localIP)
		{
			if (Socket != null)
			{
				throw new InvalidOperationException("已经开始嗅探。");
			}
			Socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
			Socket.Bind(new IPEndPoint(localIP, 0));
			byte[] inData = BitConverter.GetBytes(true);
			byte[] outData = BitConverter.GetBytes(false);
			Socket.IOControl(IOControlCode.ReceiveAll, inData, outData);
			Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, EndReceive, null);
		}

		public void Stop()
		{
			if (Socket == null)
			{
				throw new InvalidOperationException("还未开始嗅探。");
			}
			Socket.Close();
			Socket = null;
		}

		private void EndReceive(IAsyncResult result)
		{
			int length = 0;
			try
			{
				length = Socket.EndReceive(result);
			}
			catch
			{
				return;
			}
			byte[] data = Buffer;
			Buffer = new byte[ushort.MaxValue];
			Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, EndReceive, null);

			if (length < 20)
			{
				return;
			}
			if (data[2] * 256 + data[3] != length)
			{
				return;
			}

			SnifferEventArgs e = new SnifferEventArgs(data);
			if (PreviewSniffe != null)
			{
				PreviewSniffe(this, e);
				if (e.Handle)
				{
					return;
				}
			}

			SnifferPacket packet = new SnifferPacket(data.Take(length).ToArray());
			e.Packet = packet;

			if (Sniffed != null)
				Sniffed(this, e);
		}

		public event EventHandler<SnifferEventArgs> PreviewSniffe;
		public event EventHandler<SnifferEventArgs> Sniffed;
	}

	public enum Protocol
	{
		Icmp = 1,
		Igmp = 2,
		Ggp = 3,
		IP = 4,
		Tcp = 6,
		Pup = 12,
		Udp = 17,
		Idp = 22,
		ND = 77,
		Other = -1
	}
}
