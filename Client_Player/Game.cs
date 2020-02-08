using Main_Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        public Game(Player player, Room room, Roomslist parentFrm)
        {
            InitializeComponent();
            InGamePlayers = new List<Player>();
            Watchers = new List<Player>();
            Owner = player;
            InGamePlayers.Add(Owner);
            Room = room;
            parentForm = parentFrm;
        }

        private void Game_FormClosing(object sender, FormClosingEventArgs e)
        {            
            parentForm.Show();
        }
    }
}
