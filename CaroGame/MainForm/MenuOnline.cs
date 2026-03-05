using System;
using System.Windows.Forms;

namespace Client.MainForm
{
    public partial class MenuOnline : Form
    {
        public MenuOnline()
        {
            InitializeComponent();
        }

        private void btnRoomLobby_Click(object sender, EventArgs e)
        {
            var lobby = new RoomLobby();
            lobby.Show();
            this.Hide();
        }

        private void btnGroupChat_Click(object sender, EventArgs e)
        {
            var chat = new ChatForm();
            chat.Show();
        }

        private void MenuOnline_FormClosing(object sender, FormClosingEventArgs e)
        {
            var formGame = new CaroGames();
            formGame.Show();
        }
    }
}
