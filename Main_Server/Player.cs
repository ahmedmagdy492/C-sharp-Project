using Newtonsoft.Json;
using System.Net.Sockets;

namespace Main_Server
{
    public enum PlayerStatus
    {
        Waiting = 1,
        Playing = 2,
        Watching = 3
    }

    public class Player
    {
        public string Id { get; set; }
        public string PlayerName { get; set; }
        public string msgType { get; set; }
        
        public string Message { get; set; }
        [JsonIgnore]
        public PlayerStatus Status { get; set; }
        [JsonIgnore]
        public Socket PlayerSocket { get; set; }

        public Player()
        {
            Status = PlayerStatus.Waiting;
        }
    }
}