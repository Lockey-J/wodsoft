using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Wodsoft.Net.Communication
{
	public interface ICommunication
	{
		bool IsConnected {get;}

		void Send(byte[] data, byte[] head);
		void Disconnect();

		event EventHandler<CommunicationReceiveEventArgs> PreviewReceive;
		event EventHandler<CommunicationReceiveEventArgs> ReceiveCompleted;
		event EventHandler<CommunicationSendEventArgs> SendCompleted;
		event EventHandler<CommunicationDisconnectEventArgs> DisconnectCompleted;

		object this[string key] {get; set;}
		IPEndPoint EndPoint {get;}
	}
}
