using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Client.MainForm
{
    public class Online : Form
    {
        #region Designer Code
        private Label lbTime;
        private Button BackBtn;
        private Panel panelGame;
        private Button button3;
        private Button button2;
        private Button button9;
        private Button button7;
        private Button button5;
        private Button button8;
        private Button button6;
        private Button button4;
        private Button button1;
        private Label label2;
        private Label lbPlayer2Win;
        private Label lbPlayer1Win;
        private Label label1;

        private void InitializeComponent()
        {
            this.lbTime = new System.Windows.Forms.Label();
            this.BackBtn = new System.Windows.Forms.Button();
            this.panelGame = new System.Windows.Forms.Panel();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.lbPlayer2Win = new System.Windows.Forms.Label();
            this.lbPlayer1Win = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panelGame.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbTime
            // 
            this.lbTime.AutoSize = true;
            this.lbTime.Location = new System.Drawing.Point(154, 53);
            this.lbTime.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbTime.Name = "lbTime";
            this.lbTime.Size = new System.Drawing.Size(20, 24);
            this.lbTime.TabIndex = 22;
            this.lbTime.Text = "0";
            // 
            // BackBtn
            // 
            this.BackBtn.BackColor = System.Drawing.Color.SkyBlue;
            this.BackBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BackBtn.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.BackBtn.Location = new System.Drawing.Point(104, 373);
            this.BackBtn.Name = "BackBtn";
            this.BackBtn.Size = new System.Drawing.Size(134, 41);
            this.BackBtn.TabIndex = 21;
            this.BackBtn.Text = "Back";
            this.BackBtn.UseVisualStyleBackColor = false;
            // 
            // panelGame
            // 
            this.panelGame.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panelGame.Controls.Add(this.button3);
            this.panelGame.Controls.Add(this.button2);
            this.panelGame.Controls.Add(this.button9);
            this.panelGame.Controls.Add(this.button7);
            this.panelGame.Controls.Add(this.button5);
            this.panelGame.Controls.Add(this.button8);
            this.panelGame.Controls.Add(this.button6);
            this.panelGame.Controls.Add(this.button4);
            this.panelGame.Controls.Add(this.button1);
            this.panelGame.Location = new System.Drawing.Point(34, 80);
            this.panelGame.Name = "panelGame";
            this.panelGame.Size = new System.Drawing.Size(276, 287);
            this.panelGame.TabIndex = 20;
            // 
            // button3
            // 
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(183, 17);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(80, 80);
            this.button3.TabIndex = 0;
            this.button3.Text = "?";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.PlayerClick);
            // 
            // button2
            // 
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(97, 17);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(80, 80);
            this.button2.TabIndex = 0;
            this.button2.Text = "?";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.PlayerClick);
            // 
            // button9
            // 
            this.button9.ForeColor = System.Drawing.Color.Black;
            this.button9.Location = new System.Drawing.Point(183, 189);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(80, 80);
            this.button9.TabIndex = 0;
            this.button9.Text = "?";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.PlayerClick);
            // 
            // button7
            // 
            this.button7.ForeColor = System.Drawing.Color.Black;
            this.button7.Location = new System.Drawing.Point(11, 189);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(80, 80);
            this.button7.TabIndex = 0;
            this.button7.Text = "?";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.PlayerClick);
            // 
            // button5
            // 
            this.button5.ForeColor = System.Drawing.Color.Black;
            this.button5.Location = new System.Drawing.Point(97, 103);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(80, 80);
            this.button5.TabIndex = 0;
            this.button5.Text = "?";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.PlayerClick);
            // 
            // button8
            // 
            this.button8.ForeColor = System.Drawing.Color.Black;
            this.button8.Location = new System.Drawing.Point(97, 189);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(80, 80);
            this.button8.TabIndex = 0;
            this.button8.Text = "?";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.PlayerClick);
            // 
            // button6
            // 
            this.button6.ForeColor = System.Drawing.Color.Black;
            this.button6.Location = new System.Drawing.Point(183, 103);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(80, 80);
            this.button6.TabIndex = 0;
            this.button6.Text = "?";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.PlayerClick);
            // 
            // button4
            // 
            this.button4.ForeColor = System.Drawing.Color.Black;
            this.button4.Location = new System.Drawing.Point(11, 103);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(80, 80);
            this.button4.TabIndex = 0;
            this.button4.Text = "?";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.PlayerClick);
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(11, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(80, 80);
            this.button1.TabIndex = 0;
            this.button1.Text = "?";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.PlayerClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(211, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 24);
            this.label2.TabIndex = 16;
            this.label2.Text = "Player 2 Score";
            // 
            // lbPlayer2Win
            // 
            this.lbPlayer2Win.AutoSize = true;
            this.lbPlayer2Win.Location = new System.Drawing.Point(261, 33);
            this.lbPlayer2Win.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbPlayer2Win.Name = "lbPlayer2Win";
            this.lbPlayer2Win.Size = new System.Drawing.Size(25, 24);
            this.lbPlayer2Win.TabIndex = 17;
            this.lbPlayer2Win.Text = "...";
            // 
            // lbPlayer1Win
            // 
            this.lbPlayer1Win.AutoSize = true;
            this.lbPlayer1Win.Location = new System.Drawing.Point(62, 33);
            this.lbPlayer1Win.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbPlayer1Win.Name = "lbPlayer1Win";
            this.lbPlayer1Win.Size = new System.Drawing.Size(25, 24);
            this.lbPlayer1Win.TabIndex = 18;
            this.lbPlayer1Win.Text = "...";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(132, 24);
            this.label1.TabIndex = 19;
            this.label1.Text = "Player 1 Score";
            // 
            // Online
            // 
            this.ClientSize = new System.Drawing.Size(350, 422);
            this.Controls.Add(this.lbTime);
            this.Controls.Add(this.BackBtn);
            this.Controls.Add(this.panelGame);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbPlayer2Win);
            this.Controls.Add(this.lbPlayer1Win);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "Online";
            this.Text = "PK Online";
            this.panelGame.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion
        public Online()
        {
            InitializeComponent();
        }
        private void PlayerClick(object sender, EventArgs e)
        {

        }
    }
}
