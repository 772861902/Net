using Net;
using System;

public partial class HandlePlayerMsg
{
	//获取分数
	//协议参数：
	//返回协议：int分数
	public void MsgGetScore(Player player, ProtocolBase protoBase)
	{
		ProtocolBytes protocolRet = new ProtocolBytes();
		protocolRet.Addstring("GetScore");
		protocolRet.AddInt(player.data.score);
		player.Send(protocolRet);
		Console.WriteLine("MsgGetScore " + player.id + player.data.score);
	}

	//增加分数
	//协议参数：
	public void MsgAddScore(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString(start, ref start);
		//处理
		player.data.score += 1;
		Console.WriteLine("MsgAddScore " + player.id + " " + player.data.score.ToString());
	}
}