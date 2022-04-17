using System;
//字节流协议模型
public class ProtocolBytes:ProtocolBase
{   //传输的字节流
	public byte[] bytes;
	//解码
	public override ProtocolBase Decode (byte[] readbuff, int start, int length)
	{
		ProtocolBytes protocol = new ProtocolBytes ();
		protocol.bytes = new byte[length];
		Array.Copy (readbuff, start, protocol.bytes, 0, length);
		return protocol;
	}
	//编码
	public override byte[] Encode()
    {
		return bytes;
    }
	//协议名称
	public override string GetName()
    {
		return GetString(0);

    }
    //协议描述
    public override string GetDesc()
    {
		string str = " ";
		if(bytes == null)
        {
			return str;
        }
		for (int i = 0; i < bytes.Length; i++)
        {
			int b = bytes [i];
			str += b.ToString () + " ";
        }
		return str;
    }
	//字节流辅助协议
	//添加字符串
	public void Addstring(string str)
    {
		Int32 len = str.Length;
		byte[] lenBytes = BitConverter.GetBytes (len);
		byte[] strBytes = System.Text.Encoding.UTF8.GetBytes (str);
		if (bytes == null)
			bytes = lenBytes.Concat(strBytes).ToArray ();
        else
           bytes = bytes.Concat(lenBytes).Concat (strBytes).ToArray ();
    }
	//从字节数组的 start处开始读取字符串
	public string GetString (int start, ref int end)
    {
		if (bytes == null)
			return " ";
		if (bytes.Length < start + sizeof(Int32))
			return " ";
	Int32 strLen = BitConverter.ToInt32(bytes, start);
		if (bytes.Length < start + sizeof(Int32) + strLen)
			return " ";
		string str= System.Text.Encoding.UTF8.GetString (bytes,start+sizeof(Int32), strLen);
		end = start + sizeof(Int32) + strLen;
		return str;
    }
	//GetString 函数重载，使得end参数可以被忽略
	public string GetString (int start)
    {
		int end = 0;
		return GetString (start, ref end);
    }
	//添加整数
	public void AddInt(int num)
    {
		byte[] numbytes = BitConverter.GetBytes (num);
		if (bytes == null)
			bytes = numbytes;
		else
			bytes = numbytes.Concat (numbytes).ToArray ();
    }
	public int GetInt(int start , ref int end)
    {
		if (bytes == null)
			return 0;
		if (bytes.Length < start + sizeof(Int32))
			return 0;
		end = start + sizeof(Int32);
		return BitConverter.ToInt32(bytes, start);

    }
	//GetInt 函数重载
	public int GetInt(int start)
    {
		int end = 0;
		return GetInt (start, ref end);
    }
	//添加浮点数
	public void AddFloat(float  num)
	{
		byte[] numbytes = BitConverter.GetBytes(num);
		if (bytes == null)
			bytes = numbytes;
		else
			bytes = numbytes.Concat(numbytes).ToArray();
	}
	public int GetFloat(int start, ref int end)
	{
		if (bytes == null)
			return 0;
		if (bytes.Length < start + sizeof(float))
			return 0;
		end = start + sizeof(float);
		return BitConverter.ToInt32(bytes, start);

	}
	//GetInt 函数重载
	public int GetFloat(int start)
	{
		int end = 0;
		return GetInt(start, ref end);
	}
}

