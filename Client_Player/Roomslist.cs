using Client_Player.Classes;
using Main_Server;
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
                List<Room> rooms = JsonConvert.DeserializeObject<List<Room>>(dataInString);
                Rooms = rooms;
                lsRooms.Items.Clear();
                foreach(Room room in Rooms)
                {
                    lsRooms.Items.Add(room.RoomName + " " + room.RoomId);
                }
            }            
            else // showing the players in the players list box
            {
                string recData = Encoding.UTF8.GetString(temp);

                if(recData.Contains("room"))
                {
                    // deserialzing the data as a room list object
                    // recieving rooms
                    Rooms = JsonConvert.DeserializeObject<List<Room>>(recData);

                    foreach (Room room in Rooms)
                    {
                        lsRooms.Items.Add(room.RoomName + " " + room.RoomId);
                    }
                }
                else
                {
                    // deserialize the coming object which is a list of players
                    AvailPlayers = JsonConvert.DeserializeObject<List<Player>>(recData);

                    foreach (Player player in AvailPlayers)
                    {
                        lsPlayers.Items.Add(player.PlayerName + " " + player.Status);
                    }
                }
                                                
            }
            //recBuffer = new byte[2048];
            Player.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, RecieveData, null);            
        }       

        private void Roomslist_Load(object sender, EventArgs e)
        {
            lblPlayerTurn.Text = Player.PlayerName;
            
            try
            {
                // receiving players info from the server
                Player.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, RecieveData, null);

                // receiving rooms info from the server
                Player.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, RecieveData, null);

            }
            catch (SocketException)
            {
                MessageBox.Show("Cannot connect because the server is off", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
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
                room.RoomName = createRoom.RoomName;       
                string data = JsonConvert.SerializeObject(room);
                sendBuffer = Encoding.Default.GetBytes(data);
                Player.PlayerSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendData_callback, null);                

                // recieving the updated rooms data from the server
                Player.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, RecieveData, null);
                this.Hide();
                Game game = new Game(Player, room, this);
                game.Show();
            }            

        }

        private void lsRooms_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(lsRooms.SelectedItems.Count == 1)
            {
                btnJoin.Enabled = true;
            }
            else
            {
                btnSend.Enabled = false;
            }
        }

        private void btnJoin_Click(object sender, EventArgs e)
        {
            // sending a special message to the server
            // to indicate that the player wants to join a room
            // we need 2 things (room id, all player info)
            Player.msgType = "Join";

            // creating a helper object to send a special kind of data to the server
            JoinMsg helper = new JoinMsg
            {
                SenderPlayer = Player,
                RoomId = lsRooms.SelectedItem.ToString().Split(' ')[1]
            };

            string sentData = JsonConvert.SerializeObject(helper);
            recBuffer = Encoding.Default.GetBytes(sentData);
            
            /// =?>
            Player.PlayerSocket.BeginSend(recBuffer, 0, recBuffer.Length, SocketFlags.None, SendJoinData, null);
        }

        private void SendJoinData(IAsyncResult result)
        {
            Player.PlayerSocket.EndSend(result);
        }
    }
}
