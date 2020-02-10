using Client_Player;
using Main_Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace guess_the_name
{
    public partial class GameFrm : Form
    {
        public string []words;
        public string currentWord = "";
        public string showCurrentWord = "";
        public Player Owner { get; set; }
        public Room Room { get; set; }
        public List<Player> InGamePlayers { get; set; }
        public List<Player> Watchers { get; set; }
        public Roomslist parentForm { get; set; }
        public byte[] recBuffer;
        public byte[] sendBuffer;
        int wrongLetter = 0;
        public GameFrm(Player player, Room room, Roomslist parentFrm)
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

        #region Game Logic
        private void readalllines()
        {
            
            string[] readlines = File.ReadAllLines("words.txt");
            words = new string[readlines.Length];
            int index = 0;
            foreach(string s in readlines)
            {
              
                words[index++] = s;

            }
            
        }
        private void randamWordChoice()
        {
            int randamIndex = (new Random()).Next(words.Length);
            currentWord = words[randamIndex];
            showCurrentWord = "";
            for(int i=0;i<currentWord.Length;i++)
            {
                showCurrentWord += "_";
            }
            displayCurrentWord();
        }
        private void displayCurrentWord()
        {
            labelShowWord.Text = "";
            for (int i = 0; i < showCurrentWord.Length; i++)
            {
                labelShowWord.Text += showCurrentWord.Substring(i, 1);
                labelShowWord.Text += " ";
            }
        }      
        private void btnA_Click(object sender, EventArgs e)
        {
            Button choice = sender as Button;//generic object
            choice.Enabled = false;
            if(currentWord.Contains(choice.Text))
            {
                char[] tempChar = showCurrentWord.ToCharArray();
                char[] findToArray = currentWord.ToCharArray();
                char guessChar = choice.Text.ElementAt(0);
                for(int i=0;i<findToArray.Length;i++)
                {
                    if(findToArray[i]==guessChar)
                    {
                        tempChar[i] = guessChar;
                    }
                }
                showCurrentWord = new string(tempChar);
                displayCurrentWord();
            }
            else
            {
                wrongLetter++;
            }
            if(showCurrentWord.Equals(currentWord))
            {
                MessageBox.Show("you win");
            }
        }        
        private void DisableAllButtons()
        {
            labelShowWord.Visible = false;
            foreach(Button btn in keysBox.Controls)
            {
                btn.Enabled = false;
            }
        }
        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            DisableAllButtons();
            lblPlayerTurn.Text = "Wating for another player";
            readalllines();
            randamWordChoice();

            // loading connected players
            //lsPlayers.Items.Add(Owner.PlayerName);

            // listening to any request coming from any other client
            // to join the room
            // here there is a problem
            Owner.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, ReceiveData, Owner.PlayerSocket);                        
        }

        #region receiving data callback
        private void ReceiveData(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            SocketError se;
            int amountOfData = socket.EndReceive(result, out se);
            if(se == SocketError.Success)
            {
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
            
            //Owner.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, ReceiveData, Owner.PlayerSocket);
        }
        #endregion

        private void GameFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parentForm.Show();
        }
    }    
    
}
