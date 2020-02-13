using Client_Player;
using Main_Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        #region Global Members
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
        #endregion 

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

            WaitForPlayer();
        }

        #region receiving data callback
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
                    // sending yes to the server
                    Owner.msgType = "yes join";
                    string data = JsonConvert.SerializeObject(Owner);
                    recBuffer = Encoding.Default.GetBytes(data);
                    socket.BeginSend(recBuffer, 0, recBuffer.Length, SocketFlags.None, Send_Callback, socket);
                }
            }

            Owner.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, ReceiveData, Owner.PlayerSocket);
        }
        #endregion

        private void Send_Callback(IAsyncResult result)
        {
            Socket socket = (Socket)result.AsyncState;
            socket.EndSend(result);
        }

        private void GameFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            parentForm.Show();
        }   
        
        private void WaitForPlayer()
        {
            var task = Task.Run(() =>
            {
                Owner.PlayerSocket.Receive(recBuffer, recBuffer.Length, SocketFlags.None);

                string dataInString = Encoding.Default.GetString(recBuffer);
                if (dataInString.Contains("wants to join?"))
                {
                    // then we shall get a dialog box
                    DialogResult reqResult = MessageBox.Show(dataInString, "Player Request", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

                    if (reqResult == DialogResult.OK)
                    {
                        // sending yes to the server
                        Owner.msgType = "yes join";
                        string data = JsonConvert.SerializeObject(Owner);
                        recBuffer = Encoding.Default.GetBytes(data);
                        Owner.PlayerSocket.BeginSend(recBuffer, 0, recBuffer.Length, SocketFlags.None, Send_Callback, Owner.PlayerSocket);
                    }
                }
            });
            var awatier = task.GetAwaiter();
        }

        private void btnReady_Click(object sender, EventArgs e)
        {
            // listening to any request coming from any other client
            // to join the room
            // here there is a problem
            //Owner.PlayerSocket.BeginReceive(recBuffer, 0, recBuffer.Length, SocketFlags.None, ReceiveData, Owner.PlayerSocket);
            
        }
    }    
    
}
