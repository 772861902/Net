using System;
using System.Collections;

public class ProtocolBase
{   //解码
	public virtual ProtocolBase Decode(byte[] readbuff, int start, int length)
	{
		return new ProtocolBase();
	}
	//编码
	public virtual byte[] Encode()
    {
		return new byte[] { };
    }
	//协议名称
	public virtual string GetName()
    {
		return " ";
    }
	//描述
	public virtual string GetDesc()
    {
		return " ";
    }
}
