using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Main_Server;
using Newtonsoft.Json;

namespace Client_Player
{
    public partial class frmLogin : Form
    {
        private Socket client_socket;        
        private IPAddress serverIP;
        private byte[] sendBuffer;
        private byte[] recieveBuffer;
        const int SERVER_PORT = 15000;

        public frmLogin()
        {
            InitializeComponent();
            client_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverIP = IPAddress.Loopback;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            // checking whether the user entered the data properly or not
            if(!string.IsNullOrEmpty(txtName.Text.Trim()))
            {
                // connecting to the main server
                Player player = new Player
                {
                    Id = Guid.NewGuid().ToString("N"),
                    PlayerName = txtName.Text,
                    Status = "Waiting"
                };
                
                client_socket.BeginConnect(new IPEndPoint(serverIP, SERVER_PORT), Connecting_Callback, player);                
                
                this.Hide();
                // showing the next form            
                Roomslist roomslist = new Roomslist(player, client_socket);
                roomslist.Show();
            }            
        }

        #region connecting to the main server
        private void Connecting_Callback(IAsyncResult result)
        {
            // preparing the data to be send
            Player objSended = (Player)result.AsyncState;
            objSended.msgType = "Send Name";
            string data = JsonConvert.SerializeObject(objSended);
            sendBuffer = Encoding.UTF8.GetBytes(data);

            // sending data to the main server
            try
            {
                client_socket.BeginSend(sendBuffer, 0, sendBuffer.Length, SocketFlags.None, SendingData_callback, null);
            }
            catch (Exception)
            {
                
            }            
        }
        #endregion

        #region sending data to the server
        private void SendingData_callback(IAsyncResult result)
        {
            try
            {
                client_socket.EndSend(result);
            }
            catch (SocketException)
            {
                Application.Exit();
            }
            
        }
        #endregion

    }
}
