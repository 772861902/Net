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
	public void MsgGetAchieve(Player player,ProtocolBase protoBase)
    {
		ProtocolBytes protocolRet = new ProtocolBytes();
		protocolRet.Addstring("GetAchieve");
		protocolRet.AddInt(player.data.win_rate);
		player.Send(protocolRet);
		Console.WriteLine("MsgGetAchieve " + player.id + player.data.score);


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

	//接收插入留言板消息
	public void MsgAddMsg(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		string protoName = protocol.GetString(start, ref start);
		string Msg = protocol.GetString(start, ref start);
		//获取时间戳
	    long  TimeStamp = Sys.GetTimeStamp();
        Console.WriteLine("玩家ID" + player.id + "插入留言板消息" + Msg);
		//构建返回协议
		ProtocolBytes protocolRet = new ProtocolBytes();
		protocolRet.Addstring("AddMsg");
		
		if(!DataMgr.instance.AddMsg(player,TimeStamp,Msg))
        //处理
        {
			protocolRet.AddInt(0);
			Console.WriteLine("[HandlePlayerMsg报告]留言插入数据库失败！");
        }
        else
        {
			protocolRet.AddInt(-1);
		}
	    player.Send(protocolRet);

	
	}

	public void MsgGetMsg(Player player, ProtocolBase protoBase)
	{
		//获取数值
		int start = 0;
		ProtocolBytes protocol = (ProtocolBytes)protoBase;
		
		string protoName = protocol.GetString(start, ref start);
		//1表示有id,0表示无id
		int num = protocol.GetInt(start, ref start);
		string id = protocol.GetString(start, ref start);
		//题目位置
		//string  position= protocol.GetString(start, ref start);
		//字符串转整形数字
		
		//暂定应该是动态的
		ProtocolBytes protoRet = new ProtocolBytes();
		protoRet = DataMgr.instance.GetMsg(num, id);
		//protoRet = DataMgr.instance.GetMsg(num,id,position);
		player.Send(protoRet);
		
		//Console.WriteLine("MsgGetScore " + player.id + player.data.score);
	}

	
}