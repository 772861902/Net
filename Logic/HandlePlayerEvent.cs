using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Logic
{      //玩家事件类
    public class HandlePlayerEvent
    {
        //上线
        public void OnLogin(Player player)
        {
            Console.WriteLine("来自HandlePlayer的消息，玩家：" + player.id + "已成功上线");
        }
        //下线
        public void OnLogout(Player player)
        {
            Console.WriteLine("来自HandlePlayer的消息，玩家：" + player.id + "已成功下线");
        }




    }
}
