using TcpNet.Net.Sockets;
using System.Text;

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

namespace server
{
	public partial class Server : System.Windows.Forms.Form
	{
		//Form 重写 Dispose，以清理组件列表。
		[System.Diagnostics.DebuggerNonUserCode()]
		protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		//Windows 窗体设计器所必需的
		private System.ComponentModel.IContainer components;

		//注意: 以下过程是 Windows 窗体设计器所必需的
		//可以使用 Windows 窗体设计器修改它。  
		//不要使用代码编辑器修改它。
		[System.Diagnostics.DebuggerStepThrough()]
		private void InitializeComponent()
		{
			this.TextBox1 = new System.Windows.Forms.TextBox();
			this.Button1 = new System.Windows.Forms.Button();
			this.Button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			//TextBox1
			//
			this.TextBox1.Location = new System.Drawing.Point(124, 115);
			this.TextBox1.Name = "TextBox1";
			this.TextBox1.Size = new System.Drawing.Size(272, 21);
			this.TextBox1.TabIndex = 0;
			//
			//Button1
			//
			this.Button1.Location = new System.Drawing.Point(487, 130);
			this.Button1.Name = "Button1";
			this.Button1.Size = new System.Drawing.Size(98, 23);
			this.Button1.TabIndex = 1;
			this.Button1.Text = "停止";
			this.Button1.UseVisualStyleBackColor = true;
			//
			//Button2
			//
			this.Button2.Location = new System.Drawing.Point(493, 69);
			this.Button2.Name = "Button2";
			this.Button2.Size = new System.Drawing.Size(120, 31);
			this.Button2.TabIndex = 2;
			this.Button2.Text = "开始";
			this.Button2.UseVisualStyleBackColor = true;
			//
			//server
			//
			this.AutoScaleDimensions = new System.Drawing.SizeF(6.0F, 12.0F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(649, 261);
			this.Controls.Add(this.Button2);
			this.Controls.Add(this.Button1);
			this.Controls.Add(this.TextBox1);
			this.Name = "server";
			this.Text = "server";
			this.ResumeLayout(false);
			this.PerformLayout();

//INSTANT C# NOTE: Converted design-time event handler wireups:
			base.Load += new System.EventHandler(server_Load);
			Button2.Click += new System.EventHandler(Button2_Click);
			Button1.Click += new System.EventHandler(Button1_Click);
		}

		internal TextBox TextBox1;
		internal Button Button1;
		internal Button Button2;
	}

}