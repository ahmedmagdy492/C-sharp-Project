using Main_Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client_Player.Classes
{
    public class JoinMsg
    {
        public Player SenderPlayer { get; set; }
        public string RoomId { get; set; }

        public JoinMsg()
        {

        }
    }
}
