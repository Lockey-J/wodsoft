using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.Net.Sockets
{
	public class SnifferEventArgs : EventArgs
	{
		public SnifferEventArgs(byte[] buffer)
		{
			this.Buffer = buffer;
		}

		private byte[] privateBuffer;
		public byte[] Buffer
		{
			get
			{
				return privateBuffer;
			}
			private set
			{
				privateBuffer = value;
			}
		}

		public bool Handle {get; set;}

		private SnifferPacket privatePacket;
		public SnifferPacket Packet
		{
			get
			{
				return privatePacket;
			}
			internal set
			{
				privatePacket = value;
			}
		}
	}
}
