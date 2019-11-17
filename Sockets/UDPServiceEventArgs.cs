using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace Wodsoft.Net.Sockets
{
	public class UDPServiceEventArgs : EventArgs
	{
		private IPEndPoint privateEndPoint;
		public IPEndPoint EndPoint
		{
			get
			{
				return privateEndPoint;
			}
			internal set
			{
				privateEndPoint = value;
			}
		}

		private SocketAsyncOperation privateOperation;
		public SocketAsyncOperation Operation
		{
			get
			{
				return privateOperation;
			}
			internal set
			{
				privateOperation = value;
			}
		}

		private byte[] privateData;
		public byte[] Data
		{
			get
			{
				return privateData;
			}
			internal set
			{
				privateData = value;
			}
		}

		private int privateDataLength;
		public int DataLength
		{
			get
			{
				return privateDataLength;
			}
			internal set
			{
				privateDataLength = value;
			}
		}
	}
}
