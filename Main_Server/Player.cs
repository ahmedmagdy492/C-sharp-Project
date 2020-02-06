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

    internal class Player
    {
        public string Id { get; set; }
        public string PlayerName { get; set; }
        public string msgType { get; set; }
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