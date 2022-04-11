using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Logic
{   [Serializable]
    public class PlayerData
    {
        public int score = 0;
        public int win = 0;
        public int fail = 0;
        public int win_rate = 0;
       
        public PlayerData()
        {
            score = 100;
            while((win+fail)!=0)
            {
                win_rate = win / (win + fail);
            }
        }
    }
}
//注册-》生成playdata内容

//登录—》new player(id,conn)
//在player关联id 与 conn
//playdata 当前只存储了分数