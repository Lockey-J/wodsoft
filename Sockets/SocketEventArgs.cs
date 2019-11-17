using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Wodsoft.Net.Sockets
{
/// <summary>
/// Socket事件参数
/// </summary>
public class SocketEventArgs : EventArgs
{
	/// <summary>
	/// 实例化Socket事件参数
	/// </summary>
	/// <param name="socket">相关Socket</param>
	/// <param name="operation">操作类型</param>
	public SocketEventArgs(ISocket socket, SocketAsyncOperation operation)
	{
		if (socket == null)
		{
			throw new ArgumentNullException("socket");
		}
		this.Socket = socket;
		this.Operation = operation;
	}

	/// <summary>
	/// 获取或设置事件相关数据。
	/// </summary>
	public byte[] Data {get; set;}

	/// <summary>
	/// 获取数据长度。
	/// </summary>
	public int DataLength
	{
		get
		{
			return ((Data == null) ? 0 : Data.Length);
		}
	}

	/// <summary>
	/// 获取事件相关Socket
	/// </summary>
	private ISocket privateSocket;
	public ISocket Socket
	{
		get
		{
			return privateSocket;
		}
		private set
		{
			privateSocket = value;
		}
	}

	/// <summary>
	/// 获取事件操作类型。
	/// </summary>
	private SocketAsyncOperation privateOperation;
	public SocketAsyncOperation Operation
	{
		get
		{
			return privateOperation;
		}
		private set
		{
			privateOperation = value;
		}
	}
}
}
