using Main_Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client_Player
{
    public partial class Game : Form
    {

        public Player Owner { get; set; }
        public Room Room { get; set; }
        public List<Player> InGamePlayers { get; set; }
        public List<Player> Watchers { get; set; }
        public Roomslist parentForm { get; set; }
        public byte[] recBuffer;
        public byte[] sendBuffer;
        
        public Game(Player player, Room room, Roomslist parentFrm)
        {
            InitializeComponent();
            InGamePlayers = new List<Player>();
            Watchers = new List<Player>();
            Owner = player;
            InGamePlayers.Add(Owner);
            Room = room;
            parentForm = parentFrm;
            recBuffer = new byte[2048];
            sendBuffer = new byte[2048];
        }

        private void Game_FormClosing(object sender, FormClosingEventArgs e)
        {            
            parentForm.Show();
        }

        private void Game_Load(object sender, EventArgs e)
        {
            // loading connected players
            lsPlayers.Items.Add(Owner.PlayerName);

            // listening to any request coming from any other client
            // to join the room
            // here there is a problem
            Owner.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, ReceiveData, Owner.PlayerSocket);
        }

        private void ReceiveData(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;            
            int amountOfData = socket.EndReceive(result);
            byte[] temp = new byte[amountOfData];
            Array.Copy(recBuffer, 0, temp, 0, temp.Length);
            string dataInString = Encoding.Default.GetString(temp);

            if (dataInString.Contains("wants to join?"))
            {
                // then we shall get a dialog box
                DialogResult reqResult = MessageBox.Show(dataInString, "Player Request", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                if (reqResult == DialogResult.OK)
                {

                }
            }
        }
    }
}
