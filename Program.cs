using System;
using System.Net;
using System.Net.Sockets;
namespace Net;
class MainClass
{
    public static void Main(String[] args)
    {

        DataMgr a = new DataMgr();
<<<<<<< HEAD:core/Program.cs
        //玩家注册测试
       // a.Register("werre", "123456");
        ServNet servNet = new ServNet();
        servNet.proto = new ProtocolBytes();
        servNet.Start("127.0.0.1", 1234);
=======
        a.Register("wfz", "123");
        ServNet serv = new ServNet();
        serv.Start("127.0.0.1", 1234);
>>>>>>> parent of 1963871 (存疑新版本服务端更新):Program.cs
        Console.ReadLine();


        while(true)
        {
            string str = Console.ReadLine();
            switch(str)
            {
                case "quit":
                    return;
            }
        }
        

        
        //Console.WriteLine("Hello World");
        ////Socket
        //Socket listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        ////Bind
        //IPAddress ipAdr = IPAddress.Parse("127.0.0.1");
        //IPEndPoint ipEp = new IPEndPoint(ipAdr, 1234);
        //listenfd.Bind(ipEp);
        ////Listen
        //listenfd.Listen(0);
        //Console.WriteLine("[服务器] 启动成功");
        //while (true)
        //{
        //    //Accept
        //    Socket connfd = listenfd.Accept();
        //    Console.WriteLine("[服务器] Accept");
        //    //Recv
        //    byte[] readBuff = new byte[1024];
        //    int count = connfd.Receive(readBuff);
        //    string str = System.Text.Encoding.UTF8.GetString(readBuff, 0, count);
        //    Console.WriteLine("[服务器接收]" + str);
        //    //Send
        //    str = System.DateTime.Now.ToString();
        //    byte[] bytes = System.Text.Encoding.Default.GetBytes("serv echo:" + str);
        //    connfd.Send(bytes);
        //}


    }
}