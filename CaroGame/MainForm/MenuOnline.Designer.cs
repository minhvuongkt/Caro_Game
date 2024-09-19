namespace Client.MainForm
{
    partial class MenuOnline
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
            this.lbCopyright = new System.Windows.Forms.LinkLabel();
            this.btnRoomChat = new System.Windows.Forms.Button();
            this.btnFindOppoent = new System.Windows.Forms.Button();
            this.btnRandomMatch = new System.Windows.Forms.Button();
            this.lbTitle = new System.Windows.Forms.Label();
            this.txtUID = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // lbCopyright
            // 
            this.lbCopyright.AutoSize = true;
            this.lbCopyright.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCopyright.LinkColor = System.Drawing.Color.Black;
            this.lbCopyright.LinkVisited = true;
            this.lbCopyright.Location = new System.Drawing.Point(87, 313);
            this.lbCopyright.Name = "lbCopyright";
            this.lbCopyright.Size = new System.Drawing.Size(172, 20);
            this.lbCopyright.TabIndex = 9;
            this.lbCopyright.TabStop = true;
            this.lbCopyright.Text = "Copyright 2024 - Miu2k3";
            // 
            // btnRoomChat
            // 
            this.btnRoomChat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRoomChat.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnRoomChat.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRoomChat.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.btnRoomChat.Location = new System.Drawing.Point(91, 238);
            this.btnRoomChat.Name = "btnRoomChat";
            this.btnRoomChat.Size = new System.Drawing.Size(168, 53);
            this.btnRoomChat.TabIndex = 8;
            this.btnRoomChat.Text = "Room Chat";
            this.btnRoomChat.UseVisualStyleBackColor = false;
            this.btnRoomChat.Click += new System.EventHandler(this.btnRoomChat_Click);
            // 
            // btnFindOppoent
            // 
            this.btnFindOppoent.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnFindOppoent.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnFindOppoent.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFindOppoent.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.btnFindOppoent.Location = new System.Drawing.Point(91, 144);
            this.btnFindOppoent.Name = "btnFindOppoent";
            this.btnFindOppoent.Size = new System.Drawing.Size(168, 53);
            this.btnFindOppoent.TabIndex = 7;
            this.btnFindOppoent.Text = "Find Opponent";
            this.btnFindOppoent.UseVisualStyleBackColor = false;
            this.btnFindOppoent.Click += new System.EventHandler(this.btnFindOppoent_Click);
            // 
            // btnRandomMatch
            // 
            this.btnRandomMatch.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRandomMatch.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnRandomMatch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRandomMatch.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRandomMatch.Location = new System.Drawing.Point(91, 75);
            this.btnRandomMatch.Name = "btnRandomMatch";
            this.btnRandomMatch.Size = new System.Drawing.Size(168, 53);
            this.btnRandomMatch.TabIndex = 6;
            this.btnRandomMatch.Text = "Random Match";
            this.btnRandomMatch.UseVisualStyleBackColor = false;
            this.btnRandomMatch.Click += new System.EventHandler(this.btnRandomMatch_Click);
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 20F);
            this.lbTitle.Location = new System.Drawing.Point(34, 9);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(278, 37);
            this.lbTitle.TabIndex = 5;
            this.lbTitle.Text = "Welcome To PK Online";
            // 
            // txtUID
            // 
            this.txtUID.Location = new System.Drawing.Point(91, 203);
            this.txtUID.Name = "txtUID";
            this.txtUID.Size = new System.Drawing.Size(168, 29);
            this.txtUID.TabIndex = 10;
            // 
            // MenuOnline
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 337);
            this.Controls.Add(this.txtUID);
            this.Controls.Add(this.lbCopyright);
            this.Controls.Add(this.btnRoomChat);
            this.Controls.Add(this.btnFindOppoent);
            this.Controls.Add(this.btnRandomMatch);
            this.Controls.Add(this.lbTitle);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MenuOnline";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "PK Online";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuOnline_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.LinkLabel lbCopyright;
        private System.Windows.Forms.Button btnRoomChat;
        private System.Windows.Forms.Button btnFindOppoent;
        private System.Windows.Forms.Button btnRandomMatch;
        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.TextBox txtUID;
    }
}