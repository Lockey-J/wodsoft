using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wodsoft.Net.Communication
{
	public class Credential
	{
		public Credential(byte[] username, byte[] password)
		{
			this.Username = username;
			this.Password = password;
		}

		private byte[] privateUsername;
		public byte[] Username
		{
			get
			{
				return privateUsername;
			}
			private set
			{
				privateUsername = value;
			}
		}

		private byte[] privatePassword;
		public byte[] Password
		{
			get
			{
				return privatePassword;
			}
			private set
			{
				privatePassword = value;
			}
		}
	}
}
