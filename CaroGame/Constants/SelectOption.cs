using Client.MainForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace Client.Constants
{
    public class SelectOption : Form
    {
        private Button btn30s;
        private Button btn20s;
        private Button btn10s;
        private Button btnQuayLai;

        public SelectOption()
        {
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ControlBox = false;

            this.Text = "Select Time Play";
            this.Size = new System.Drawing.Size(300, 200);

            btn30s = new Button();
            btn30s.Text = "30s";
            btn30s.Location = new System.Drawing.Point(30, 50);
            btn30s.Click += new EventHandler(btn30s_Click);
            this.Controls.Add(btn30s);

            btn20s = new Button();
            btn20s.Text = "20s";
            btn20s.Location = new System.Drawing.Point(110, 50);
            btn20s.Click += new EventHandler(btn20s_Click);
            this.Controls.Add(btn20s);

            btn10s = new Button();
            btn10s.Text = "10s";
            btn10s.Location = new System.Drawing.Point(190, 50);
            btn10s.Click += new EventHandler(btn10s_Click);
            this.Controls.Add(btn10s);

            btnQuayLai = new Button();
            btnQuayLai.Text = "Back";
            btnQuayLai.Location = new System.Drawing.Point(100, 100);
            btnQuayLai.Click += new EventHandler(BtnQuayLai_Click);
            this.Controls.Add(btnQuayLai);
        }
        private void btn30s_Click(object sender, EventArgs e)
        {
            var pvp = new PvP(30);
            pvp.ShowDialog();
            this.Close();
        }
        private void btn20s_Click(object sender, EventArgs e)
        {
            var pvp = new PvP(20);
            pvp.Show();
            this.Close();
        }
        private void btn10s_Click(object sender, EventArgs e)
        {
            var pvp = new PvP(10);
            pvp.Show();
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
            // SelectOption
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "SelectOption";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }
    }
}
