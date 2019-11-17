using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wodsoft.Net.Sockets;

namespace Wodsoft.Net.Communication
{
	public class ServerClient : CommunicationBase
	{
		public ServerClient(ISocket socket) : base(socket)
		{
		}

		private Credential privateCredential;
		public Credential Credential
		{
			get
			{
				return privateCredential;
			}
			internal set
			{
				privateCredential = value;
			}
		}

		private bool privateIsAuthenticated;
		public bool IsAuthenticated
		{
			get
			{
				return privateIsAuthenticated;
			}
			internal set
			{
				privateIsAuthenticated = value;
			}
		}

		public override bool IsConnected
		{
			get
			{
				return base.IsConnected && IsAuthenticated;
			}
		}
	}
}
