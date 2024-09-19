using Client.Connection;
using Client.Constants;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.MainForm
{
    public partial class CaroGames : Form
    {
        private PlayerClient plClient { get; set; }
        public CaroGames()
        {
            plClient = new PlayerClient();

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();
            var thread = new Thread(() => plClient.Receive());
            thread.IsBackground = true;
            thread.Start();
            plClient.GetPlayer();
        }
        private void btnPKOnline_Click(object sender, EventArgs e)
        {
            if (DataCache.Player == null)
            {
                string input = Interaction.InputBox("Nhập tên của bạn:", "Nhập liệu", "");
                if (!string.IsNullOrEmpty(input))
                {

                    plClient.CreateNewPlayer(input);
                    var selectOptions = new MenuSelect();
                    selectOptions.ShowDialog();

                }
                else
                {
                    MessageBox.Show("Bạn chưa nhập tên.", "Thông báo");
                }
            }
            else
            {
                var selectOptions = new MenuSelect();
                selectOptions.ShowDialog();
            }
            //this.Close();
        }

        private void btnTwoPlayer_Click(object sender, EventArgs e)
        {
            var selectOptions = new SelectOption();
            selectOptions.ShowDialog();
        }

        private void btnStart1P_Click(object sender, EventArgs e)
        {
            var selectForm = new SelectLevel();
            selectForm.ShowDialog();
        }

        private void lbCopyright_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://facebook.com/miuuu.2k3");
        }
    }
}
