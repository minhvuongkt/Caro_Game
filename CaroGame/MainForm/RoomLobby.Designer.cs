namespace Client.MainForm
{
    partial class RoomLobby
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.lvRooms      = new System.Windows.Forms.ListView();
            this.colName      = new System.Windows.Forms.ColumnHeader();
            this.colType      = new System.Windows.Forms.ColumnHeader();
            this.colStatus    = new System.Windows.Forms.ColumnHeader();
            this.colPlayers   = new System.Windows.Forms.ColumnHeader();
            this.colSpec      = new System.Windows.Forms.ColumnHeader();
            this.btnCreate    = new System.Windows.Forms.Button();
            this.btnJoinPlayer = new System.Windows.Forms.Button();
            this.btnSpectate  = new System.Windows.Forms.Button();
            this.btnRefresh   = new System.Windows.Forms.Button();
            this.btnGroupChat = new System.Windows.Forms.Button();
            this.btnMatchQueue = new System.Windows.Forms.Button();
            this.btnBack      = new System.Windows.Forms.Button();
            this.lblTitle     = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lvRooms
            this.lvRooms.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colName, this.colType, this.colStatus, this.colPlayers, this.colSpec });
            this.lvRooms.FullRowSelect = true;
            this.lvRooms.GridLines = true;
            this.lvRooms.Location = new System.Drawing.Point(12, 55);
            this.lvRooms.Name = "lvRooms";
            this.lvRooms.Size = new System.Drawing.Size(760, 300);
            this.lvRooms.TabIndex = 0;
            this.lvRooms.UseCompatibleStateImageBehavior = false;
            this.lvRooms.View = System.Windows.Forms.View.Details;
            this.lvRooms.MultiSelect = false;

            this.colName.Text = "Tên phòng";    this.colName.Width = 260;
            this.colType.Text = "Game";         this.colType.Width = 110;
            this.colStatus.Text = "Trạng thái"; this.colStatus.Width = 100;
            this.colPlayers.Text = "Người chơi"; this.colPlayers.Width = 100;
            this.colSpec.Text = "Khán giả";     this.colSpec.Width = 90;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 16F);
            this.lblTitle.Location = new System.Drawing.Point(12, 14);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Phòng chơi Online";

            int bh = 40, by = 370, bx = 12;

            // btnCreate (110 wide)
            this.btnCreate.Text = "Tạo phòng";
            this.btnCreate.Location = new System.Drawing.Point(bx, by); bx += 118;
            this.btnCreate.Size = new System.Drawing.Size(110, bh);
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);

            // btnJoinPlayer
            this.btnJoinPlayer.Text = "Vào chơi";
            this.btnJoinPlayer.Location = new System.Drawing.Point(bx, by); bx += 118;
            this.btnJoinPlayer.Size = new System.Drawing.Size(110, bh);
            this.btnJoinPlayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJoinPlayer.Click += new System.EventHandler(this.btnJoinPlayer_Click);

            // btnSpectate
            this.btnSpectate.Text = "Xem";
            this.btnSpectate.Location = new System.Drawing.Point(bx, by); bx += 118;
            this.btnSpectate.Size = new System.Drawing.Size(110, bh);
            this.btnSpectate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSpectate.Click += new System.EventHandler(this.btnSpectate_Click);

            // btnRefresh
            this.btnRefresh.Text = "Làm mới";
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Location = new System.Drawing.Point(bx, by); bx += 118;
            this.btnRefresh.Size = new System.Drawing.Size(110, bh);
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);

            // btnGroupChat
            this.btnGroupChat.Text = "Chat chung";
            this.btnGroupChat.Location = new System.Drawing.Point(bx, by); bx += 118;
            this.btnGroupChat.Size = new System.Drawing.Size(110, bh);
            this.btnGroupChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGroupChat.Click += new System.EventHandler(this.btnGroupChat_Click);

            // btnMatchQueue — random matchmaking (amber colour via UITheme)
            this.btnMatchQueue.Text = "Ghép trận ngẫu nhiên";
            this.btnMatchQueue.Name = "btnMatchQueue";
            this.btnMatchQueue.Location = new System.Drawing.Point(bx, by); bx += 188;
            this.btnMatchQueue.Size = new System.Drawing.Size(180, bh);
            this.btnMatchQueue.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMatchQueue.Click += new System.EventHandler(this.btnMatchQueue_Click);

            // btnBack
            this.btnBack.Text = "Quay lại";
            this.btnBack.Name = "btnBack";
            this.btnBack.Location = new System.Drawing.Point(bx, by);
            this.btnBack.Size = new System.Drawing.Size(110, bh);
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 422);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                this.lblTitle, this.lvRooms,
                this.btnCreate, this.btnJoinPlayer, this.btnSpectate,
                this.btnRefresh, this.btnGroupChat, this.btnMatchQueue, this.btnBack });
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "RoomLobby";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Phòng chơi Online";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RoomLobby_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ListView lvRooms;
        private System.Windows.Forms.ColumnHeader colName, colType, colStatus, colPlayers, colSpec;
        private System.Windows.Forms.Button btnCreate, btnJoinPlayer, btnSpectate, btnRefresh, btnGroupChat, btnMatchQueue, btnBack;
        private System.Windows.Forms.Label lblTitle;
    }
}
