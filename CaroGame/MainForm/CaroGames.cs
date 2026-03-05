using Client.Connection;
using Client.Constants;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Client.MainForm
{
    public partial class CaroGames : Form
    {
        private PlayerClient plClient { get; set; }
        public CaroGames()
        {
            plClient = new PlayerClient();   // connects + registers "Player" handler
            // Also init GameClient singleton so it registers its handlers before the dispatcher starts
            var _ = GameClient.Instance;

            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();

            // Start single central dispatcher (replaces per-client receive threads)
            MessageDispatcher.Instance.Start();
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
                    selectOptions.Show();
                }
                else
                {
                    MessageBox.Show("Bạn chưa nhập tên.", "Thông báo");
                }
            }
            else
            {
                var selectOptions = new MenuSelect();
                selectOptions.Show();
            }
            this.Hide();
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

        private void CaroGames_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
