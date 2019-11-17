using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Wodsoft.Net.Communication
{
	public abstract class CommunicationEventArgs : EventArgs
	{
		public CommunicationEventArgs(ICommunication communication)
		{
			if (communication == null)
			{
				throw new ArgumentNullException("communication");
			}
			this.Communication = communication;
		}

		private ICommunication privateCommunication;
		public ICommunication Communication
		{
			get
			{
				return privateCommunication;
			}
			private set
			{
				privateCommunication = value;
			}
		}
	}

	public class CommunicationAcceptEventArgs : CommunicationEventArgs
	{
		public CommunicationAcceptEventArgs(ICommunication communication, byte[] head, Credential credential) : base(communication)
		{
			this.Head = head;
			Handled = false;
			this.Credential = credential;
		}

		private byte[] privateHead;
		public byte[] Head
		{
			get
			{
				return privateHead;
			}
			private set
			{
				privateHead = value;
			}
		}

		public bool Handled {get; set;}

		public byte[] FailedData {get; set;}

		private Credential privateCredential;
		public Credential Credential
		{
			get
			{
				return privateCredential;
			}
			private set
			{
				privateCredential = value;
			}
		}
	}

	public class CommunicationConnectEventArgs : CommunicationEventArgs
	{
		public CommunicationConnectEventArgs(ICommunication communication, bool success, byte[] failedData) : base(communication)
		{
			this.Success = success;
			this.FailedData = failedData;
		}

		private bool privateSuccess;
		public bool Success
		{
			get
			{
				return privateSuccess;
			}
			private set
			{
				privateSuccess = value;
			}
		}

		private byte[] privateFailedData;
		public byte[] FailedData
		{
			get
			{
				return privateFailedData;
			}
			private set
			{
				privateFailedData = value;
			}
		}
	}

	public class CommunicationReceiveEventArgs : CommunicationEventArgs
	{
		public CommunicationReceiveEventArgs(ICommunication communication, Guid dataID, int dataLength, byte[] head) : base(communication)
		{
			this.DataID = dataID;
			this.DataLength = dataLength;
			this.Head = head;
			Handled = false;
		}

		private Guid privateDataID;
		public Guid DataID
		{
			get
			{
				return privateDataID;
			}
			private set
			{
				privateDataID = value;
			}
		}

		private int privateDataLength;
		public int DataLength
		{
			get
			{
				return privateDataLength;
			}
			private set
			{
				privateDataLength = value;
			}
		}

		private byte[] privateHead;
		public byte[] Head
		{
			get
			{
				return privateHead;
			}
			private set
			{
				privateHead = value;
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

		public bool Handled {get; set;}

		private bool privateSuccess;
		public bool Success
		{
			get
			{
				return privateSuccess;
			}
			internal set
			{
				privateSuccess = value;
			}
		}

		public byte[] FailedData {get; set;}
	}

	public class CommunicationSendEventArgs : CommunicationEventArgs
	{
		public CommunicationSendEventArgs(ICommunication communication, byte[] data, byte[] head) : base(communication)
		{
			DataID = Guid.NewGuid();
			this.Data = data;
			this.Head = head;
		}

		public bool Handled {get; set;}

		private Guid privateDataID;
		public Guid DataID
		{
			get
			{
				return privateDataID;
			}
			private set
			{
				privateDataID = value;
			}
		}

		private byte[] privateHead;
		public byte[] Head
		{
			get
			{
				return privateHead;
			}
			private set
			{
				privateHead = value;
			}
		}

		private byte[] privateData;
		public byte[] Data
		{
			get
			{
				return privateData;
			}
			private set
			{
				privateData = value;
			}
		}

		private bool privateSuccess;
		public bool Success
		{
			get
			{
				return privateSuccess;
			}
			internal set
			{
				privateSuccess = value;
			}
		}

		private byte[] privateFailedData;
		public byte[] FailedData
		{
			get
			{
				return privateFailedData;
			}
			internal set
			{
				privateFailedData = value;
			}
		}
	}

	public class CommunicationDisconnectEventArgs : CommunicationEventArgs
	{
		public CommunicationDisconnectEventArgs(ICommunication communication, byte[] data) : base(communication)
		{
			this.Data = data;
		}

		private byte[] privateData;
		public byte[] Data
		{
			get
			{
				return privateData;
			}
			private set
			{
				privateData = value;
			}
		}
	}
}