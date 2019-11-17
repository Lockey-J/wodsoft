using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Wodsoft.Net.Sockets
{
	public class SnifferPacket
	{
		private int HeadLength;

		public SnifferPacket(byte[] rawData)
		{
			if (rawData.Length < 20)
			{
				throw new ArgumentException("无效的包数据。");
			}
			if (rawData[2] * 256 + rawData[3] != rawData.Length)
			{
				throw new ArgumentException("无效的包数据。");
			}
			Time = DateTime.Now;
			this.RawData = rawData;
			HeadLength = (rawData[0] & 0xF) * 4;
		}

		private DateTime privateTime;
		public DateTime Time
		{
			get
			{
				return privateTime;
			}
			private set
			{
				privateTime = value;
			}
		}

		private byte[] privateRawData;
		public byte[] RawData
		{
			get
			{
				return privateRawData;
			}
			private set
			{
				privateRawData = value;
			}
		}

		public IPEndPoint Source
		{
			get
			{
				IPEndPoint ip = new IPEndPoint(new IPAddress(BitConverter.ToUInt32(RawData, 12)),0);
				if (Protocol == Sockets.Protocol.Tcp || Protocol == Sockets.Protocol.Udp)
				{
					ip.Port = RawData[HeadLength] * 256 + RawData[HeadLength + 1];
				}
				return ip;
			}
		}

		public IPEndPoint Destination
		{
			get
			{
				IPEndPoint ip = new IPEndPoint(new IPAddress(BitConverter.ToUInt32(RawData, 16)), 0);
				if (Protocol == Sockets.Protocol.Tcp || Protocol == Sockets.Protocol.Udp)
				{
					ip.Port = RawData[HeadLength + 2] * 256 + RawData[HeadLength + 3];
				}
				return ip;
			}
		}

		public Protocol Protocol
		{
            get
            {
                bool flag = Enum.IsDefined(typeof(Protocol), (int)this.RawData[9]);
                Protocol Protocol;
                if (flag)
                {
                    Protocol = (Protocol)this.RawData[9];
                }
                else
                {
                    Protocol = Protocol.Other;
                }
                return Protocol;
            }
        }

		public byte[] Data
		{
			get
			{
				return RawData.Skip(HeadLength + 8).ToArray();
			}
		}

		public int Version
		{
			get
			{
				return (RawData[0] & 0xF0) >> 4;
			}
		}
	}
}
