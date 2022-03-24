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



namespace Net
{
    public class DataMgr
    {
        int a;
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
            string id = "SwordsMan";  //用户名
            string pwd = "123456";  
            string database = "SwordsMan";   //数据库名
            string connStr = string.Format("Server = {0}; Database = {1}; User ID = {2}; Password = {3};", host, database, id, pwd);
         
       
            sqlConn = new MySqlConnection(connStr);
            try
            {
                sqlConn.Open();
                Console.WriteLine("[DataMgr] :Connect Successful! ");
            }
            catch(Exception e)
            {
                Console.WriteLine("[DataMgr] : Connect Error! " + e.Message);
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
			string cmdStr = string.Format("select * from user where id='{0}';", id);
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
				Console.WriteLine("[DataMgr]CanRegister fail " + e.Message);
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
			string cmdStr = string.Format("insert into Users set Username ='{0}' ,Password ='{1}';", id, pw);
			MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
			try
			{
				cmd.ExecuteNonQuery();
				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine("[DataMgr]Register " + e.Message);
				return false;
			}
		}
        public bool Checkpassword(string id, string pw)
        {
            string cmdStr = string.Format("select * from Users where Username = '{0}' and  Password = '{1}';", id, pw);
            MySqlCommand cmd = new MySqlCommand(cmdStr, sqlConn);
            try
            {
                MySqlDataReader dataReader = cmd.ExecuteReader();
                bool hasRows = dataReader.HasRows;
                dataReader.Close();
                return hasRows;
            }
            catch (Exception e)
            {
                Console.WriteLine("[DataMgr] CheckPassword" + e.Message);
                return false;
            }

        }

    }
}
