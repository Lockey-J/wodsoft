//INSTANT C# NOTE: Formerly VB project-level imports:
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using Wodsoft.Net.Sockets;

namespace client
{
	public partial class client
	{
		public client()
		{
			InitializeComponent();
		}


		private TCPClient client_Conflict;
		private delegate void myde(byte[] str);
		private void client_Load(object sender, EventArgs e)
		{
			client_Conflict = new TCPClient();

			client_Conflict.ReceiveCompleted += Receive;
		

		}

		private void Button1_Click(object sender, EventArgs e)
		{
			if (client_Conflict.IsConnected)
			{
				client_Conflict.Disconnect();
			}
			client_Conflict.Connect(new System.Net.IPEndPoint(System.Net.IPAddress.Parse(TextBox1.Text), 1314));
			TextBox2.Text = "开始链接：" + client_Conflict.RemoteEndPoint.ToString() + "链接状态：" + client_Conflict.IsConnected.ToString();


		}
		private void settextbox(byte[] str)
		{
			string m = System.Text.Encoding.UTF8.GetString(str);
			TextBox2.Text = TextBox2.Text + Environment.NewLine + "接收到数据：" + m;

		}
		private void Receive(object sender, SocketEventArgs e)
		{
		
			byte[] data = BitConverter.GetBytes(DateTime.Now.TimeOfDay.TotalMilliseconds);
			myde m = new myde(settextbox);
			BeginInvoke(m, e.Data);
			//m.Invoke(e.Data())
		}

		private void Button2_Click(object sender, EventArgs e)
		{
			byte[] data = System.Text.Encoding.UTF8.GetBytes("客户端请求" + DateTime.Now.ToString());
			client_Conflict.SendAsync(data);

		}

		private static client _DefaultInstance;
		public static client DefaultInstance
		{
			get
			{
				if (_DefaultInstance == null || _DefaultInstance.IsDisposed)
					_DefaultInstance = new client();

				return _DefaultInstance;
			}
		}
	}

}