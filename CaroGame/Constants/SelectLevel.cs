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
            this.Text = "Chọn độ khó Bot";
            this.Size = new System.Drawing.Size(360, 180);

            var lbl = new System.Windows.Forms.Label
            {
                Text = "Chọn chế độ chơi với Bot:",
                AutoSize = true,
                Location = new System.Drawing.Point(20, 20),
                Font = new System.Drawing.Font("Segoe UI Semibold", 12F)
            };
            this.Controls.Add(lbl);

            btnKho = new Button { Text = "Bình thường", Size = new System.Drawing.Size(130, 40), Location = new System.Drawing.Point(20, 65), FlatStyle = FlatStyle.Flat };
            btnDe  = new Button { Text = "Dễ (Ngốc)",  Size = new System.Drawing.Size(130, 40), Location = new System.Drawing.Point(165, 65), FlatStyle = FlatStyle.Flat };
            btnQuayLai = new Button { Text = "Quay lại", Size = new System.Drawing.Size(120, 40), Location = new System.Drawing.Point(115, 115), FlatStyle = FlatStyle.Flat, Name = "btnBack" };

            btnKho.Click += new EventHandler(BtnKho_Click);
            btnDe.Click  += new EventHandler(BtnDe_Click);
            btnQuayLai.Click += new EventHandler(BtnQuayLai_Click);

            this.Controls.Add(btnKho);
            this.Controls.Add(btnDe);
            this.Controls.Add(btnQuayLai);

            UITheme.Apply(this);
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
