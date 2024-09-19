using Client.Connection;
using Client.MainForm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.Constants
{
    public class MenuSelect : Form
    {
        private Button btnMenuPK;
        private Button btnBack;
        private Button btnMySelf;
        public MenuSelect()
        {
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.btnMenuPK = new System.Windows.Forms.Button();
            this.btnMySelf = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnMenuPK
            // 
            this.btnMenuPK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMenuPK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnMenuPK.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMenuPK.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.btnMenuPK.Location = new System.Drawing.Point(207, 12);
            this.btnMenuPK.Name = "btnMenuPK";
            this.btnMenuPK.Size = new System.Drawing.Size(168, 53);
            this.btnMenuPK.TabIndex = 9;
            this.btnMenuPK.Text = "Menu PK Online";
            this.btnMenuPK.UseVisualStyleBackColor = false;
            this.btnMenuPK.Click += new System.EventHandler(this.btnMenuPK_Click);
            // 
            // btnMySelf
            // 
            this.btnMySelf.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnMySelf.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnMySelf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMySelf.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMySelf.Location = new System.Drawing.Point(12, 12);
            this.btnMySelf.Name = "btnMySelf";
            this.btnMySelf.Size = new System.Drawing.Size(168, 53);
            this.btnMySelf.TabIndex = 8;
            this.btnMySelf.Text = "My Infomation";
            this.btnMySelf.UseVisualStyleBackColor = false;
            this.btnMySelf.Click += new System.EventHandler(this.btnMySelf_Click);
            // 
            // btnBack
            // 
            this.btnBack.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnBack.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnBack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBack.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.btnBack.Location = new System.Drawing.Point(113, 73);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(168, 53);
            this.btnBack.TabIndex = 9;
            this.btnBack.Text = "Back";
            this.btnBack.UseVisualStyleBackColor = false;
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            // 
            // MenuSelect
            // 
            this.ClientSize = new System.Drawing.Size(387, 138);
            this.ControlBox = false;
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnMenuPK);
            this.Controls.Add(this.btnMySelf);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MenuSelect";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);

        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            var formGame = new CaroGames();
            formGame.Show();
            this.Close();
        }

        private void btnMenuPK_Click(object sender, EventArgs e)
        {            
            var menupk = new MenuOnline();
            menupk.Show();
            this.Close();
        }

        private void btnMySelf_Click(object sender, EventArgs e)
        {
            var info = new Infomation();
            info.Show();
            //this.Close();
        }
    }
}
