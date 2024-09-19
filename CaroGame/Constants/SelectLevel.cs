using Client.MainForm;
using System;
using System.Windows.Forms;

namespace Client.Constants
{
    public class SelectLevel : Form
    {
        private Button btnKho;
        private Button btnDe;
        private Button btnQuayLai;

        public SelectLevel()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;

            this.Text = "Choose Mode Bot";
            this.Size = new System.Drawing.Size(300, 200);

            btnKho = new Button();
            btnKho.Text = "Normal";
            btnKho.Location = new System.Drawing.Point(50, 50);
            btnKho.Click += new EventHandler(BtnKho_Click);
            this.Controls.Add(btnKho);

            btnDe = new Button();
            btnDe.Text = "Stupid";
            btnDe.Location = new System.Drawing.Point(150, 50);
            btnDe.Click += new EventHandler(BtnDe_Click);
            this.Controls.Add(btnDe);

            btnQuayLai = new Button();
            btnQuayLai.Text = "Back";
            btnQuayLai.Location = new System.Drawing.Point(100, 100);
            btnQuayLai.Click += new EventHandler(BtnQuayLai_Click);
            this.Controls.Add(btnQuayLai);
        }
        private void BtnKho_Click(object sender, EventArgs e)
        {
            var plBot = new PlayBot(1);
            plBot.Show();
            this.Close();
        }
        private void BtnDe_Click(object sender, EventArgs e)
        {
            var plBot = new PlayBot(0);
            plBot.Show();
            this.Close();
        }
        private void BtnQuayLai_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SelectLevel
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SelectLevel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
    }
}
