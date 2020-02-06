using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Main_Server
{
    class Program
    {
        private static Socket server_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        const int PORT = 15000;
        private static List<Player> ConnectedPlayers = new List<Player>();
        private static byte[] sendingBuffer;
        private static byte[] receivingBuffer;

        static void Main(string[] args)
        {
            // start listening and accepting clients requests
            server_socket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            server_socket.Listen(5);
            sendingBuffer = new byte[1024];
            receivingBuffer = new byte[2048];
            
            server_socket.BeginAccept(AcceptingClients_callback, null);
            Console.ReadKey();
        }

        #region Begin accepting clients
        static void AcceptingClients_callback(IAsyncResult result)
        {
            // getting client socket
            Socket socket = server_socket.EndAccept(result);

            // begin receiving data from the client
            socket.BeginReceive(receivingBuffer, 0, sendingBuffer.Length, SocketFlags.None, ReceivingData_callback, socket);

            // accepting new client
            server_socket.BeginAccept(AcceptingClients_callback, null);
        }
        #endregion

        #region Begin Receiving data from the clients
        static void ReceivingData_callback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            int amountOfData = socket.EndReceive(result);            

            // creating a new player
            byte[] temp = new byte[amountOfData];
            Array.Copy(receivingBuffer, 0, temp, 0, temp.Length);

            // checking the type of the incoming message
            string data = Encoding.UTF8.GetString(temp);
            if (data.Contains("Send Name"))
            {
                // then the player is sending an object along with his name
                // so we will cast the data to Player object
                Player player = JsonConvert.DeserializeObject<Player>(data);
                // adding the captured socket to the player socket
                player.PlayerSocket = socket;
                // adding the player to the connected players list
                ConnectedPlayers.Add(player);

                // sending some data to all clients
                string dataTobeSend = JsonConvert.SerializeObject(ConnectedPlayers);
                sendingBuffer = Encoding.UTF8.GetBytes(dataTobeSend);
                player.PlayerSocket.BeginSend(sendingBuffer, 0, sendingBuffer.Length, SocketFlags.None, SendingData_callback, player.PlayerSocket);
            }
            else if(data.Contains("Send Msg"))
            {
                // it means that the player wants to send a message to other players 
                // in the chat room
                foreach(Player player in ConnectedPlayers)
                {
                    byte[] tmp = Encoding.UTF8.GetBytes(data);
                    player.PlayerSocket.BeginSend(tmp, 0, tmp.Length, SocketFlags.None, SendingData_callback, player.PlayerSocket);
                }
                Player sender = JsonConvert.DeserializeObject<Player>(data);
                Console.WriteLine($"{sender.PlayerName}: {sender.msgType.Split(':')[1]}");
            }

            // showing the data on the console screen
            foreach(Player p in ConnectedPlayers)
            {
                Console.WriteLine($"playerId: {p.Id}\n playerName: {p.PlayerName} \n playerStatus:  {p.Status} \n PlayerSocket: {p.PlayerSocket.ToString()}");
            }
            // begin receiving data from the client
            socket.BeginReceive(receivingBuffer, 0, sendingBuffer.Length, SocketFlags.None, ReceivingData_callback, socket);
        }
        #endregion

        #region sending data to all clients
        private static void SendingData_callback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndSend(result);
        }
        #endregion

    }
}
