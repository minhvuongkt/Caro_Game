using Client.Connection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Constants
{
    public partial class Infomation : Form
    {
        public Infomation()
        {
            InitializeComponent();
            ShowUp();
        }
        void ShowUp()
        {            
            txtFullname.Text = DataCache.Player.Fullname;
            txtUid.Text = DataCache.Player.UID;
            lbScore.Text = DataCache.Player.Score.ToString();
        }
    }
}
