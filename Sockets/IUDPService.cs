using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Wodsoft.Net.Sockets
{
	public interface IUDPService
	{
		void Send(IPEndPoint remote, byte[] data);
		void SendAsync(IPEndPoint remote, byte[] data);
		event EventHandler<UDPServiceEventArgs> ReceiveCompleted;
		event EventHandler<UDPServiceEventArgs> SendCompleted;
		IPEndPoint LocalEndPoint {get;}
	}
}
