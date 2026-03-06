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
            this.Text = "Chọn thời gian mỗi lượt";
            this.Size = new System.Drawing.Size(360, 180);

            var lbl = new System.Windows.Forms.Label
            {
                Text = "Thời gian mỗi lượt:",
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20),
                Font = new System.Drawing.Font("Segoe UI Semibold", 12F)
            };
            this.Controls.Add(lbl);

            btn30s = new Button { Text = "30 giây", Size = new System.Drawing.Size(90, 40), Location = new System.Drawing.Point(20,  60), FlatStyle = FlatStyle.Flat };
            btn20s = new Button { Text = "20 giây", Size = new System.Drawing.Size(90, 40), Location = new System.Drawing.Point(130, 60), FlatStyle = FlatStyle.Flat };
            btn10s = new Button { Text = "10 giây", Size = new System.Drawing.Size(90, 40), Location = new System.Drawing.Point(240, 60), FlatStyle = FlatStyle.Flat };
            btnQuayLai = new Button { Text = "Quay lại", Size = new System.Drawing.Size(120, 40), Location = new System.Drawing.Point(115, 115), FlatStyle = FlatStyle.Flat, Name = "btnBack" };

            btn30s.Click += new EventHandler(btn30s_Click);
            btn20s.Click += new EventHandler(btn20s_Click);
            btn10s.Click += new EventHandler(btn10s_Click);
            btnQuayLai.Click += new EventHandler(BtnQuayLai_Click);

            this.Controls.Add(btn30s);
            this.Controls.Add(btn20s);
            this.Controls.Add(btn10s);
            this.Controls.Add(btnQuayLai);

            UITheme.Apply(this);
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
