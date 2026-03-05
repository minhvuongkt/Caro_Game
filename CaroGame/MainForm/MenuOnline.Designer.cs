namespace Client.MainForm
{
    partial class MenuOnline
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lbTitle       = new System.Windows.Forms.Label();
            this.btnRoomLobby  = new System.Windows.Forms.Button();
            this.btnGroupChat  = new System.Windows.Forms.Button();
            this.lbCopyright   = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();

            // lbTitle
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 20F);
            this.lbTitle.Location = new System.Drawing.Point(25, 12);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Text = "PK Online";

            // btnRoomLobby
            this.btnRoomLobby.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRoomLobby.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnRoomLobby.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRoomLobby.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.btnRoomLobby.Location = new System.Drawing.Point(75, 80);
            this.btnRoomLobby.Name = "btnRoomLobby";
            this.btnRoomLobby.Size = new System.Drawing.Size(200, 53);
            this.btnRoomLobby.TabIndex = 1;
            this.btnRoomLobby.Text = "Phòng chơi Online";
            this.btnRoomLobby.UseVisualStyleBackColor = false;
            this.btnRoomLobby.Click += new System.EventHandler(this.btnRoomLobby_Click);

            // btnGroupChat
            this.btnGroupChat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnGroupChat.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnGroupChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGroupChat.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.btnGroupChat.Location = new System.Drawing.Point(75, 154);
            this.btnGroupChat.Name = "btnGroupChat";
            this.btnGroupChat.Size = new System.Drawing.Size(200, 53);
            this.btnGroupChat.TabIndex = 2;
            this.btnGroupChat.Text = "Chat chung / Bạn bè";
            this.btnGroupChat.UseVisualStyleBackColor = false;
            this.btnGroupChat.Click += new System.EventHandler(this.btnGroupChat_Click);

            // lbCopyright
            this.lbCopyright.AutoSize = true;
            this.lbCopyright.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lbCopyright.Location = new System.Drawing.Point(75, 260);
            this.lbCopyright.Name = "lbCopyright";
            this.lbCopyright.Text = "Copyright 2024 - Miu2k3";

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 295);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lbTitle, this.btnRoomLobby, this.btnGroupChat, this.lbCopyright });
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MenuOnline";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PK Online";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuOnline_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Button btnRoomLobby;
        private System.Windows.Forms.Button btnGroupChat;
        private System.Windows.Forms.LinkLabel lbCopyright;
    }
}
