using Main_Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace Client_Player
{
    public partial class Roomslist : Form
    {
        public Player Player{ get; set; }
        public byte[] recBuffer { get; set; }
        public byte[] sendBuffer { get; set; }
        public List<Player> AvailPlayers { get; set; }


        public Roomslist(string playerName, Socket socket)
        {
            InitializeComponent();
            Player = new Player { PlayerName = playerName, PlayerSocket = socket };
            recBuffer = new byte[1024];
            sendBuffer = new byte[1024];
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
                lsChat.Items.Add($"{player.PlayerName}: {player.msgType.Split(':')[1]}");
            }
            else
            {
                // deserialize the coming object which is a list
                AvailPlayers = JsonConvert.DeserializeObject<List<Player>>(Encoding.UTF8.GetString(temp));

                // showing the data on the list box
                foreach (Player player in AvailPlayers)
                {
                    lsPlayers.Items.Add(player.PlayerName + " " + player.Status);
                }
            }
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
                Player.msgType = $"Send Msg:{txtMsg.Text}";
                
                string objStr = JsonConvert.SerializeObject(Player);
                sendBuffer = Encoding.UTF8.GetBytes(objStr);
                Player.PlayerSocket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendData_callback, null);
            }
        }

        private void SendData_callback(IAsyncResult result)
        {
            Player.PlayerSocket.EndSend(result);
        }
    }
}
