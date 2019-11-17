using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Wodsoft.Net.Sockets
{
	internal class UDPServiceAsyncState : SocketAsyncState
	{
		public IPEndPoint EndPoint {get; set;}
	}
}
