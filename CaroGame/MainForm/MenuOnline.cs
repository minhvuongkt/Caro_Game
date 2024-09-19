using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.MainForm
{
    public partial class MenuOnline : Form
    {
        public MenuOnline()
        {
            InitializeComponent();
        }

        private void btnRandomMatch_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Chưa phát triển ní ơi :3");
        }

        private void btnFindOppoent_Click(object sender, EventArgs e)
        {

        }

        private void btnRoomChat_Click(object sender, EventArgs e)
        {
            var chatt = new ChatForm();
            chatt.Show();
        }

        private void MenuOnline_FormClosing(object sender, FormClosingEventArgs e)
        {
        }
    }
}
