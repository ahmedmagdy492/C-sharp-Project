using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Client_Player.Classes;
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
        private static List<Room> Rooms = new List<Room>();
        static JoinMsg recMsg;

        static void Main(string[] args)
        {
            // start listening and accepting clients requests
            server_socket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            server_socket.Listen(5);
            sendingBuffer = new byte[2048];
            receivingBuffer = new byte[2048];
            
            server_socket.BeginAccept(AcceptingClients_callback, null);
            Console.ReadKey();

            // closing all clients
            foreach(Player player in ConnectedPlayers)
            {
                player.PlayerSocket.Shutdown(SocketShutdown.Both);
            }            
        }

        #region Begin accepting clients
        static void AcceptingClients_callback(IAsyncResult result)
        {
            // getting client socket
            Socket socket = server_socket.EndAccept(result);

            // begin receiving data from clients
            socket.BeginReceive(receivingBuffer, 0, receivingBuffer.Length, SocketFlags.None, ReceivingData_callback, socket);
            

            // accepting new client
            server_socket.BeginAccept(AcceptingClients_callback, null);
        }
        #endregion                

        #region Begin Receiving data from the clients
        static void ReceivingData_callback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            int amountOfData = 0;
            // we are handling wether there is user that is disconnected
            try
            {
                amountOfData = socket.EndReceive(result);
            }
            catch(SocketException)
            {
                Console.WriteLine("===============================================");
                Console.WriteLine(socket.RemoteEndPoint + " Disconnected");
                Console.WriteLine("===============================================");
                
            }
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

                // sending some connected players data to all clients
                string dataTobeSend = JsonConvert.SerializeObject(ConnectedPlayers);
                sendingBuffer = Encoding.UTF8.GetBytes(dataTobeSend);
                player.PlayerSocket.BeginSend(sendingBuffer, 0, sendingBuffer.Length, SocketFlags.None, SendingData_callback, player.PlayerSocket);

                // sending rooms data to the client
                //string roomsData = JsonConvert.SerializeObject(Rooms);
                //sendingBuffer = Encoding.Default.GetBytes(roomsData);
                //player.PlayerSocket.BeginSend(sendingBuffer, 0, sendingBuffer.Length, SocketFlags.None, SendingData_callback, player.PlayerSocket);
            }
            else if(data.Contains("Send Msg"))
            {
                // it means that the player wants to send a message to other players 
                // in the chat room
                foreach(Player player in ConnectedPlayers)
                {
                    byte[] tmp = Encoding.Default.GetBytes(data);
                    Console.WriteLine(data);
                    player.PlayerSocket.BeginSend(tmp, 0, tmp.Length, SocketFlags.None, SendingMsg_callback, player.PlayerSocket);
                }
                
                Player sender = JsonConvert.DeserializeObject<Player>(data);
                
            }
            else if(data.Contains("Create Room"))
            {
                // getting data from a client that wants to create a room
                Room room = JsonConvert.DeserializeObject<Room>(data);
                foreach(Player p in ConnectedPlayers)
                {
                    if(p.PlayerSocket == socket)
                    {
                        room.OwnerPlayer = p;
                    }
                }
                Rooms.Add(room);

                // serializing the room list to be send
                string roomsListStr = JsonConvert.SerializeObject(Rooms);

                // sending created rooms info to all players                
                foreach(Player player in ConnectedPlayers)
                {
                    byte[] tmp = Encoding.Default.GetBytes(roomsListStr);                    
                    player.PlayerSocket.BeginSend(tmp, 0, tmp.Length, SocketFlags.None, SendingData_callback, player.PlayerSocket);
                }
                Console.WriteLine($"Room {room.RoomName} created by");
            }
            else if(data.Contains("Join"))
            {
                // receveing the data from a client that wants to join a room
                recMsg = JsonConvert.DeserializeObject<JoinMsg>(data);

                // checking the room status
                foreach (Room room in Rooms)
                {
                    if (room.RoomId == recMsg.RoomId)
                    {
                        if (room.Players.Count == 1)
                        {
                            string msg = $"{recMsg.SenderPlayer.PlayerName} wants to join?";
                            byte[] tmp = Encoding.Default.GetBytes(msg);
                            room.OwnerPlayer.PlayerSocket.BeginSend(tmp, 0, tmp.Length ,SocketFlags.None, SendingJoinReq_callback, room.OwnerPlayer.PlayerSocket);                            
                        }
                    }
                }

                // if is full true we will allow him to enter the room as a player
                // otherwise we will show a message
                Console.WriteLine(recMsg.SenderPlayer.PlayerName + " Wants to join room " + recMsg.RoomId);

                //TODO: check if the room is full with 2 players or not
                // if so => send a request msg to the room owner 
                // otherwise => allow him to enter the room as a watcher                
            }

            // showing the data on the console screen
            foreach (Player p in ConnectedPlayers)
            {
                Console.WriteLine($"playerId: {p.Id}\n playerName: {p.PlayerName} \n playerStatus:  {p.Status} \n PlayerSocket: {p.PlayerSocket.ToString()}");
            }
            // begin receiving data from the client
            try
            {
                socket.BeginReceive(receivingBuffer, 0, receivingBuffer.Length, SocketFlags.None, ReceivingData_callback, socket);
            }
            catch(SocketException)
            {
                Console.WriteLine("===============================================");
                Console.WriteLine(socket.RemoteEndPoint + " Disconnected");
                Console.WriteLine("===============================================");
                // removing the disconnected client from the players list
                foreach (Player player in ConnectedPlayers)
                {
                    if (player.PlayerSocket == socket)
                    {
                        ConnectedPlayers.Remove(player);
                        break;
                    }
                }
            }
        }       

        private static void SendingJoinReq_callback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            SocketError se;
            socket.EndSend(ar, out se);
            Console.WriteLine(se.ToString());
        }
        #endregion

        #region sending data to all clients
        private static void SendingData_callback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndSend(result);

            //socket.BeginSend(receivingBuffer, 0, receivingBuffer.Length, SocketFlags.None, SendingData_callback, socket);
        }
        #endregion


        private static void SendingMsg_callback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndSend(result);            

            //socket.BeginSend(receivingBuffer, 0, receivingBuffer.Length, SocketFlags.None, SendingData_callback, socket);
        }
    }
}
