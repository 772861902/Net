using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Net.Logic;

namespace Net
{
    public class DataMgr
    {
        MySqlConnection sqlConn;
        //单例模式
        public static DataMgr instance;
        public DataMgr()
        {
            instance = this;
            Connect();
        }

        public void Connect()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);   //防止出现字符编码错误，包括依懒项中引入的system.text.* 均为了解决此问题。
            string host = "127.0.0.1";
            string id = "root";  //用户名
            string pwd = "123456"; //密码
            string database = "game";  //数据库名
            string connStr = string.Format("Server = {0}; Database = {1}; User ID = {2}; Password = {3};", host, database, id, pwd);


            sqlConn = new MySqlConnection(connStr);
            try
            {
                sqlConn.Open();
                Console.WriteLine("[DataMgr] :Connect Successful! 连接数据成功！");
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr] : Connect Error!连接数据库失败！ " + e.Message);
                return;
            }
        }

        //判定安全字符串
        public bool IsSafeStr(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        //是否存在该用户
        private bool CanRegister(string id)
        {
            //防sql注入
            if (!IsSafeStr(id))
                return false;
            //查询id是否存在
            string cmdStr = string.Format("select * from players where ID='{0}';", id);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return !hasRows;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]CanRegister fail! 注册用户已存在！无法注册！ " + e.Message);
                return false;
            }
        }

        //注册
        public bool Register(string id, string pw)
        {
            //防sql注入
            if (!IsSafeStr(id) || !IsSafeStr(pw))
            {
                Console.WriteLine("[DataMgr]Register 使用非法字符");
                return false;
            }
            //能否注册
            if (!CanRegister(id))
            {
                Console.WriteLine("[DataMgr]Register !CanRegister");
                return false;
            }
            //写入数据库User表
            string cmdStr = string.Format("insert into players set ID ='{0}' ,PassWord ='{1}';", id, pw);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("[DataMgr]注册成功,注册玩家ID:"+id);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]Register " + e.Message);
                return false;
            }
        }
        //验证密码
        public bool Checkpassword(string id, string pw)
        {
            string cmdStr = string.Format("select * from players where ID = '{0}' and  PassWord = '{1}';", id, pw);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                //if(hasRows)
                //Console.WriteLine("[DataMgr] 密码正确！");
                return hasRows;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr] 密码错误！CheckPassword" + e.Message);
                return false;
            }

        }
        //角色数据
       
        //创建角色
        public bool CreatePlayer(string id) 
        {
            //防SQL注入
            //if(IsSafeStr(id))
              //  return false;
            //序列化
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            PlayerData playerData = new PlayerData();
            try
            {
                formatter.Serialize(stream, playerData);
            }
            catch(Exception e)
            {
                Console.WriteLine("[DataMgr]CreatePlayer PlayerData序列化" + e.Message);
                return false;
            }
            byte[] byteArr = stream.ToArray();
            //写入数据库
            string cmdStr = string.Format("insert into players set ID ='{0}',PlayerData=@data;", id);
            MySqlCommand cmd = new MySqlCommand(cmdStr,sqlConn);
            cmd.Parameters.Add("@data", MySqlDbType.Blob);
            cmd.Parameters[0].Value = byteArr;
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine("[DataMgr]CreatePlayer 写入" + e.Message);
                return false;
            }

        }
        
        //获取玩家数据
        public PlayerData GetPlayerData(string id)
        {
            PlayerData playerData = null;
            //防SQL注入
            if (!IsSafeStr(id))
                return playerData;
            //查询
            //？？？？？？？？？？？？why playerData???

            string cmdStr = string.Format("select * from players where ID ='{0}';", id);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            byte[] buffer = new byte[1];
            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                Console.WriteLine("获取玩家数据");
                if (!dataReader.HasRows)
                {
                    Console.WriteLine("玩家数据为空");
                    dataReader.Close();
                    return playerData;
                }
                dataReader.Read();
                //player第十一列是playerdata,11-1=10
                long len = dataReader.GetBytes(10, 0, null, 0, 0);//
                buffer = new byte[len];
                dataReader.GetBytes(10, 0, buffer, 0, (int)len);
                dataReader.Close();
            }
            catch (Exception e)

            {
                Console.WriteLine("[DataMgr]GetPlayerData 查询" + e.Message);
                return playerData;
            }
            //反序列化
            MemoryStream stream = new MemoryStream(buffer);
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                playerData = (PlayerData)formatter.Deserialize(stream);
                return playerData;
            }
            catch (SerializationException e)
            {
                Console.WriteLine("[DataMgr]GetPlayerData反序列化"+e.Message);
                return playerData;
            }
        }

            //保存角色
            public bool SavePlayer(Player player)
           {
            string id = player.id;
             PlayerData playerData = player.data;
            //序列化
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            try
            {
                 formatter.Serialize(stream, playerData);

            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]SavePlayer 序列化" + e.Message);
                return false;
            }
            byte[] byteArr = stream.ToArray();
            //写入数据库
            string formatStr = "update players set PlayerData = @data where ID = '{0}';";
            string cmdStr = string.Format(formatStr, player.id);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            cmd.Parameters.Add("@data", MySqlDbType.Blob);
            cmd.Parameters[0].Value = byteArr;
            try
            {
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]SavePlayer 写入 " + e.Message);
                return false;
            }
        }


        //保存角色
        /*public bool AddMsg(Player player,long TimeStamp,String Msg)
      {   
          string id = player.id;
          //PlayerData playerData = player.data;
          //序列化
          IFormatter formatter = new BinaryFormatter();
          MemoryStream stream = new MemoryStream();
          try
          {
               formatter.Serialize(stream, Msg);

          }
          catch (Exception e)
          {
              Console.WriteLine("[DataMgr]AddMsg 序列化" + e.Message);
              return false;
          }
          byte[] byteArr = stream.ToArray();
          //写入数据库
          string formatStr = String.Format("insert player_message wherer ID='{0}',TimeStamp='{1}',Message=@data;"); 
          string cmdStr = string.Format(formatStr, player.id,TimeStamp);
          MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
          cmd.Parameters.Add("@data", MySqlDbType.Blob);
          cmd.Parameters[0].Value = byteArr;
          try
          {
              cmd.ExecuteNonQuery();
              return true;
          }
          catch (Exception e)
          {
              Console.WriteLine("[DataMgr]SavePlayer 写入 " + e.Message);
              return false;
          }
      }  */
        public bool AddMsg(Player player, long TimeStamp, String Msg)
        {


            string cmdStr = string.Format("insert player_message set ID='{0}',TimeStamp='{1}',Message='{2}';", player.id,TimeStamp,Msg);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try
            {
                cmd.ExecuteNonQuery();
                Console.WriteLine("[DataMgr]插入用户留言成功:" + player.id+Msg);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr]AddMsg " + e.Message);
                return false;
            }

        }

        public ProtocolBytes GetMsg(int num,string id)
        {
            String cmdStr;
            ProtocolBytes protocol = new ProtocolBytes();
            protocol.Addstring("GetMsg");

            //暂时未考虑题目
            if (num == 0)
            {
                 cmdStr = "select * from player_message limit 10;";
            }
            else
            {
                 cmdStr = string.Format("select * from player_message where ID='{0}' limit 10;", id);
            }
          
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            //byte[] buffer = new byte[1];
            try
            {
                MySqlDataReader MsgReader = cmd.ExecuteReader();
                //Console.WriteLine("");
                if (!MsgReader.HasRows)
                {
                    Console.WriteLine("数据库相关内容查询为空");
                    MsgReader.Close();
                    return protocol;
                }
                int MsgRowsCount = MsgReader.GetSchemaTable().Rows.Count;
                
                protocol.AddInt(MsgRowsCount);
                string str="";
                while (MsgReader.Read())
                {
                    string TimeStamp = MsgReader.GetString(0);
                    string MsgContent = MsgReader.GetString(2);
                    //将获取的时间戳字符串转换为数字
                    int TimeStamptemp = int.Parse(TimeStamp);
                    //将时间戳转换为日期格式
                    DateTime datetime = Sys.GetDateTime(TimeStamptemp);
                    //将日期格式转换为字符串
                    string TimeStamp1 = datetime.ToString();
                    string ID = MsgReader.GetString(1);
                    
                    protocol.Addstring(ID);
                    protocol.Addstring(MsgContent);
                    protocol.Addstring(TimeStamp1);
                    str=str+TimeStamp1+ID+ MsgContent;
                }
                MsgReader.Close();
                Console.WriteLine("[DataMgr]RetMsg中的消息"+str);
                //  long len = dataReader.GetBytes(0, 0, null, 0, 0);//1是Data?

                return protocol;


            }
            catch (Exception e)

            {
              
                Console.WriteLine("[DataMgr][GetMsg]获取失败！"+e.Message);
                 return protocol;
            }
          
        }

    }
}
