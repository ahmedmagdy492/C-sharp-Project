﻿using Main_Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Client_Player
{
    public partial class Roomslist : Form
    {
        public Player Player{ get; set; }
        public byte[] recBuffer { get; set; }
        public byte[] sendBuffer { get; set; }
        public List<Player> AvailPlayers { get; set; }
        public List<Room> Rooms = new List<Room>();
        public delegate void ShowItems();
        public ShowItems showItems;
        public Roomslist thisFrm;

        public Roomslist(Player player, Socket socket)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            Player = player;
            Player.PlayerSocket = socket;
            recBuffer = new byte[2048];
            sendBuffer = new byte[2048];
            thisFrm = this;            
        }        

        private void RecieveData(IAsyncResult result)
        {
            int amountOfdata = Player.PlayerSocket.EndReceive(result);
            byte[] temp = new byte[amountOfdata];
            Array.Copy(recBuffer, 0, temp, 0, temp.Length);
            string dataInString = Encoding.UTF8.GetString(temp);            
            
            // checking what type of response has the server sent
            if(dataInString.Contains("Send Msg"))
            {
                // then it will be a normal msg that we need to display on the chat for everyone
                Player player = JsonConvert.DeserializeObject<Player>(dataInString);

                for(int i = 0; i < AvailPlayers.Count; i++ )
                {
                    if (AvailPlayers[i].Id == player.Id)
                    {
                        AvailPlayers[i].Message = player.Message;
                    }
                }                
                lsChat.Items.Add($"{player.PlayerName} : {player.Message}");
            }
            else if(dataInString.Contains("Create Room"))
            {
                Room room = JsonConvert.DeserializeObject<Room>(dataInString);                
                Rooms.Add(room);
                lsRooms.Items.Add(room.RoomName);
            }
            else // showing the players in the players list box
            {
                // deserialize the coming object which is a list
                AvailPlayers = JsonConvert.DeserializeObject<List<Player>>(Encoding.UTF8.GetString(temp));

                foreach (Player player in AvailPlayers)
                {
                    lsPlayers.Items.Add(player.PlayerName + " " + player.Status);
                }                                
            }
            //recBuffer = new byte[2048];
            Player.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, RecieveData, null);
        }       

        private void Roomslist_Load(object sender, EventArgs e)
        {
            lblPlayerTurn.Text = Player.PlayerName;

            // receiving some info from the server
            Player.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, RecieveData, null);
        }

        private void Roomslist_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.OpenForms[0].Close();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtMsg.Text.Trim()))
            {
                // sending a message to the server which is going to be send to all clients
                Player.msgType = $"Send Msg";
                Player.Message = txtMsg.Text;                

                string objStr = JsonConvert.SerializeObject(Player);                
                sendBuffer = Encoding.UTF8.GetBytes(objStr);                
                Player.PlayerSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendData_callback, null);
            }
        }

        private void SendData_callback(IAsyncResult result)
        {
            Player.PlayerSocket.EndSend(result);
            //Player.PlayerSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendData_callback, null);
        }

        private void btnCreateRoom_Click(object sender, EventArgs e)
        {
            // showing the new room window
            CreateRoom createRoom = new CreateRoom();
            DialogResult result = createRoom.ShowDialog();

            // checking on the returned form value
            if(result == DialogResult.OK)
            {                
                // creating a new room by sending a request to the server
                Player.msgType = "Create Room";
                Room room = new Room(Player);
                room.MsgType = "Create Room";
                room.RoomId = Guid.NewGuid().ToString("N");
                room.RoomName = "Hamdy";       
                string data = JsonConvert.SerializeObject(room);
                sendBuffer = Encoding.Default.GetBytes(data);
                Player.PlayerSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendData_callback, null);                

                // recieving the updated rooms data from the server
                Player.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, RecieveData, null);
            }            

        }        
    }
}
