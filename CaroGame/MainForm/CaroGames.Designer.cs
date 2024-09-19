namespace Client.MainForm
{
    partial class CaroGames
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaroGames));
            this.lbTitle = new System.Windows.Forms.Label();
            this.btnStart1P = new System.Windows.Forms.Button();
            this.btnTwoPlayer = new System.Windows.Forms.Button();
            this.btnPKOnline = new System.Windows.Forms.Button();
            this.lbCopyright = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lbTitle
            // 
            this.lbTitle.AutoSize = true;
            this.lbTitle.Font = new System.Drawing.Font("Segoe UI Semilight", 20F);
            this.lbTitle.Location = new System.Drawing.Point(25, 9);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(296, 37);
            this.lbTitle.TabIndex = 0;
            this.lbTitle.Text = "Welcome To Caro Game";
            // 
            // btnStart1P
            // 
            this.btnStart1P.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnStart1P.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnStart1P.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStart1P.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart1P.Location = new System.Drawing.Point(86, 105);
            this.btnStart1P.Name = "btnStart1P";
            this.btnStart1P.Size = new System.Drawing.Size(168, 53);
            this.btnStart1P.TabIndex = 1;
            this.btnStart1P.Text = "Play With Bot";
            this.btnStart1P.UseVisualStyleBackColor = false;
            this.btnStart1P.Click += new System.EventHandler(this.btnStart1P_Click);
            // 
            // btnTwoPlayer
            // 
            this.btnTwoPlayer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnTwoPlayer.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnTwoPlayer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTwoPlayer.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.btnTwoPlayer.Location = new System.Drawing.Point(86, 174);
            this.btnTwoPlayer.Name = "btnTwoPlayer";
            this.btnTwoPlayer.Size = new System.Drawing.Size(168, 53);
            this.btnTwoPlayer.TabIndex = 2;
            this.btnTwoPlayer.Text = "2 Player";
            this.btnTwoPlayer.UseVisualStyleBackColor = false;
            this.btnTwoPlayer.Click += new System.EventHandler(this.btnTwoPlayer_Click);
            // 
            // btnPKOnline
            // 
            this.btnPKOnline.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnPKOnline.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnPKOnline.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPKOnline.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.btnPKOnline.Location = new System.Drawing.Point(86, 244);
            this.btnPKOnline.Name = "btnPKOnline";
            this.btnPKOnline.Size = new System.Drawing.Size(168, 53);
            this.btnPKOnline.TabIndex = 3;
            this.btnPKOnline.Text = "PK Online";
            this.btnPKOnline.UseVisualStyleBackColor = false;
            this.btnPKOnline.Click += new System.EventHandler(this.btnPKOnline_Click);
            // 
            // lbCopyright
            // 
            this.lbCopyright.AutoSize = true;
            this.lbCopyright.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCopyright.LinkColor = System.Drawing.Color.Black;
            this.lbCopyright.LinkVisited = true;
            this.lbCopyright.Location = new System.Drawing.Point(82, 343);
            this.lbCopyright.Name = "lbCopyright";
            this.lbCopyright.Size = new System.Drawing.Size(172, 20);
            this.lbCopyright.TabIndex = 4;
            this.lbCopyright.TabStop = true;
            this.lbCopyright.Text = "Copyright 2024 - Miu2k3";
            this.lbCopyright.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lbCopyright_LinkClicked);
            // 
            // CaroGames
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 372);
            this.Controls.Add(this.lbCopyright);
            this.Controls.Add(this.btnPKOnline);
            this.Controls.Add(this.btnTwoPlayer);
            this.Controls.Add(this.btnStart1P);
            this.Controls.Add(this.lbTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "CaroGames";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Caro Game";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbTitle;
        private System.Windows.Forms.Button btnStart1P;
        private System.Windows.Forms.Button btnTwoPlayer;
        private System.Windows.Forms.Button btnPKOnline;
        private System.Windows.Forms.LinkLabel lbCopyright;
    }
}