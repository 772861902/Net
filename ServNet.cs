using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections;

namespace Net
{
    public class ServNet
    {
        //监听套接字
        public Socket lisetnfd;
        //客户端连接
        public Conn[] conns;
        //最大连接数
        public int maxConn = 50;

        public static ServNet instance;

        public ServNet()
        {
            instance = this;
        }

        //获得最大连接池索引，返回负数表示失败
        public int NewIndex()
        {
            if (conns == null)
                return -1;
            for(int i = 0;i<conns.Length;i++)
            {
                if(conns[i] == null)
                {
                    conns[i] = new Conn();
                    return i;
                }
                else if(conns[i].isUse ==false)
                {
                    return i;
                }
            }
            return -1;
        }

        //开启服务器
        public void Start(string host,int port)
        {
            //连接池
            conns = new Conn[maxConn];
            for(int i = 0;i<maxConn;i++)
            {
                conns[i] = new Conn();

            }
            //Socket
            lisetnfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Bind
            IPAddress ipAdr = IPAddress.Parse(host);
            IPEndPoint ipEp = new IPEndPoint(ipAdr, port);
            lisetnfd.Bind(ipEp);
            //Listen
            lisetnfd.Listen(maxConn);
            //Accept
            lisetnfd.BeginAccept(AcceptCb,null);
            Console.WriteLine("[服务器] 启动成功");
        }
        
        //Accept回调
        private void AcceptCb(IAsyncResult ar)
        {
            try
            {
                Socket socket = lisetnfd.EndAccept(ar);
                int index = NewIndex();
                
                if(index < 0)
                {
                    socket.Close();
                    Console.WriteLine("[警告] 连接已满");

                }
                else
                {
                    Conn conn = conns[index];
                    conn.Init(socket);
                    string adr = conn.GetAddress();
                    Console.WriteLine("客户端连接[" + adr + "] conn池ID : " + index);
                    conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);
                    
                }
                lisetnfd.BeginAccept(AcceptCb, null);


            }
            catch(Exception e)
            {
                Console.WriteLine("AcceptCb失败：" + e.Message);
            }
        }

        private void ReceiveCb(IAsyncResult ar)
        {
            Conn conn = (Conn)ar.AsyncState;
            try
            {
                int count = conn.socket.EndReceive(ar);
                //关闭信号
                if(count <= 0)
                {
                    Console.WriteLine("收到[" + conn.GetAddress() + "]断开连接");
                    conn.Close();
                    return;
                }
                //数据处理
                string str = System.Text.Encoding.UTF8.GetString(conn.readBuff, 0, count);
                Console.WriteLine("收到[" + conn.GetAddress() + "]数据" + str);
                str = conn.GetAddress() + ":" + str;
                byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
                //广播给所有正在使用的连接
                for(int i = 0;i<conns.Length;i++)
                {
                    if (conns[i] == null)
                        continue;
                    if (!conns[i].isUse)
                        continue;
                    Console.WriteLine("将消息转发给:" + conns[i].GetAddress());
                    conns[i].socket.Send(bytes);
                }
                //继续接收，
                conn.socket.BeginReceive(conn.readBuff, conn.buffCount, conn.BuffRemain(), SocketFlags.None, ReceiveCb, conn);


            }
            catch(Exception e)
            {
                Console.WriteLine("收到[" + conn.GetAddress() + "]断开连接");
                conn.Close();
            }
        }

        public void Close()
        {
            for(int i = 0; i < conns.Length;i++)
            {
                Conn conn = conns[i];
                if (conn == null) continue;
                if (!conn.isUse) continue;
                lock(conn)
                {
                    conn.Close();
                }
            }
        }

    }
}
