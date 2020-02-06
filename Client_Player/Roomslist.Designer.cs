namespace Client_Player
{
    partial class Roomslist
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblPlayerTurn = new System.Windows.Forms.Label();
            this.chatPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lsPlayers = new System.Windows.Forms.ListBox();
            this.lsRooms = new System.Windows.Forms.ListBox();
            this.btnJoin = new System.Windows.Forms.Button();
            this.btnCreateRoom = new System.Windows.Forms.Button();
            this.lsChat = new System.Windows.Forms.ListBox();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.chatPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnCreateRoom);
            this.panel1.Controls.Add(this.btnJoin);
            this.panel1.Controls.Add(this.lblPlayerTurn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(915, 76);
            this.panel1.TabIndex = 0;
            // 
            // lblPlayerTurn
            // 
            this.lblPlayerTurn.AutoSize = true;
            this.lblPlayerTurn.Location = new System.Drawing.Point(23, 29);
            this.lblPlayerTurn.Name = "lblPlayerTurn";
            this.lblPlayerTurn.Size = new System.Drawing.Size(88, 20);
            this.lblPlayerTurn.TabIndex = 0;
            this.lblPlayerTurn.Text = "Player Turn";
            // 
            // chatPanel
            // 
            this.chatPanel.Controls.Add(this.lsChat);
            this.chatPanel.Controls.Add(this.txtMsg);
            this.chatPanel.Controls.Add(this.btnSend);
            this.chatPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.chatPanel.Location = new System.Drawing.Point(0, 76);
            this.chatPanel.Name = "chatPanel";
            this.chatPanel.Size = new System.Drawing.Size(200, 385);
            this.chatPanel.TabIndex = 1;
            // 
            // lsPlayers
            // 
            this.lsPlayers.Dock = System.Windows.Forms.DockStyle.Left;
            this.lsPlayers.FormattingEnabled = true;
            this.lsPlayers.ItemHeight = 20;
            this.lsPlayers.Location = new System.Drawing.Point(200, 76);
            this.lsPlayers.Name = "lsPlayers";
            this.lsPlayers.Size = new System.Drawing.Size(164, 385);
            this.lsPlayers.TabIndex = 2;
            // 
            // lsRooms
            // 
            this.lsRooms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsRooms.FormattingEnabled = true;
            this.lsRooms.ItemHeight = 20;
            this.lsRooms.Location = new System.Drawing.Point(364, 76);
            this.lsRooms.Name = "lsRooms";
            this.lsRooms.Size = new System.Drawing.Size(551, 385);
            this.lsRooms.TabIndex = 3;
            // 
            // btnJoin
            // 
            this.btnJoin.Location = new System.Drawing.Point(813, 18);
            this.btnJoin.Name = "btnJoin";
            this.btnJoin.Size = new System.Drawing.Size(90, 42);
            this.btnJoin.TabIndex = 1;
            this.btnJoin.Text = "Join";
            this.btnJoin.UseVisualStyleBackColor = true;
            // 
            // btnCreateRoom
            // 
            this.btnCreateRoom.Location = new System.Drawing.Point(674, 18);
            this.btnCreateRoom.Name = "btnCreateRoom";
            this.btnCreateRoom.Size = new System.Drawing.Size(120, 42);
            this.btnCreateRoom.TabIndex = 2;
            this.btnCreateRoom.Text = "Create Room";
            this.btnCreateRoom.UseVisualStyleBackColor = true;
            // 
            // lsChat
            // 
            this.lsChat.FormattingEnabled = true;
            this.lsChat.ItemHeight = 20;
            this.lsChat.Location = new System.Drawing.Point(3, 3);
            this.lsChat.Name = "lsChat";
            this.lsChat.Size = new System.Drawing.Size(197, 244);
            this.lsChat.TabIndex = 0;
            // 
            // txtMsg
            // 
            this.txtMsg.Location = new System.Drawing.Point(3, 253);
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(197, 26);
            this.txtMsg.TabIndex = 1;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(3, 285);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(197, 43);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // Roomslist
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 461);
            this.Controls.Add(this.lsRooms);
            this.Controls.Add(this.lsPlayers);
            this.Controls.Add(this.chatPanel);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Roomslist";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Roomslist";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Roomslist_FormClosing);
            this.Load += new System.EventHandler(this.Roomslist_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.chatPanel.ResumeLayout(false);
            this.chatPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label lblPlayerTurn;
        private System.Windows.Forms.FlowLayoutPanel chatPanel;
        private System.Windows.Forms.ListBox lsPlayers;
        private System.Windows.Forms.ListBox lsRooms;
        private System.Windows.Forms.Button btnJoin;
        private System.Windows.Forms.Button btnCreateRoom;
        private System.Windows.Forms.ListBox lsChat;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.Button btnSend;
    }
}