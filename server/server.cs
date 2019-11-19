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
using TcpNet.Net.Sockets;
using System.Text;

namespace server
{
	public partial class Server
	{
		public Server()
		{
			InitializeComponent();
		}

		private delegate void myde(byte[] str);
		private TCPListener listener;
		private void server_Load(object sender, EventArgs e)
		{
			listener = new TCPListener();

			listener.Port = 1314;
			listener.ReceiveCompleted += Listener_ReceiveCompleted;
			listener.Start();


		}
		private void settextbox(byte[] str)
		{
			string m = System.Text.Encoding.UTF8.GetString(str);
			TextBox1.Text = m;
		}
		private void Listener_ReceiveCompleted(object sender, SocketEventArgs e)
		{
			e.Socket.SendAsync(Encoding.UTF8.GetBytes(e.Socket.RemoteEndPoint.ToString()));
			myde md = new myde(settextbox);
			this.BeginInvoke(md, e.Data);
		}

		private void Button2_Click(object sender, EventArgs e)
		{
            try
            {
				listener.Start();
                

            }
			catch (Exception)
            {

			}

		}

		private void Button1_Click(object sender, EventArgs e)
		{
			listener.Stop();
		}

		private static Server _DefaultInstance;
		public static Server DefaultInstance
		{
			get
			{
				if (_DefaultInstance == null || _DefaultInstance.IsDisposed)
					_DefaultInstance = new Server();

				return _DefaultInstance;
			}
		}
	}

}