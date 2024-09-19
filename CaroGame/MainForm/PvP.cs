using Client.Constants;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Client.MainForm
{
    public partial class PvP : Form
    {
        Button btnDelete { get; set; } = new Button();
        private bool isButtonVisible { get; set; }
        int player1Wons { get; set; } = 0;
        int player2Wons { get; set; } = 0;
        int timeSet { get; set; } = 20;
        int timeLeft { get; set; }
        List<Button> buttons { get; set; }
        List<Button> btnMoves { get; set; }
        Player currPlayer { get; set; }
        public PvP(int time = 20)
        {
            timeSet = time;
            timeLeft = timeSet;
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            RestartGames();
        }
        private void CheckGames()
        {
            if ((button1.Text == "X" && button2.Text == "X" && button3.Text == "X")
                || (button4.Text == "X" && button5.Text == "X" && button6.Text == "X")
                || (button7.Text == "X" && button8.Text == "X" && button9.Text == "X")
                || (button1.Text == "X" && button4.Text == "X" && button7.Text == "X")
                || (button2.Text == "X" && button5.Text == "X" && button8.Text == "X")
                || (button3.Text == "X" && button6.Text == "X" && button9.Text == "X")
                || (button1.Text == "X" && button5.Text == "X" && button9.Text == "X")
                || (button3.Text == "X" && button5.Text == "X" && button7.Text == "X"))
            {
                MessageBox.Show("Player 1 Win", "Thông Báo");
                player1Wons++;
                lbPlayer1Win.Text = player1Wons.ToString();
                RestartGames();
            }
            else if ((button1.Text == "O" && button2.Text == "O" && button3.Text == "O")
                || (button4.Text == "O" && button5.Text == "O" && button6.Text == "O")
                || (button7.Text == "O" && button8.Text == "O" && button9.Text == "O")
                || (button1.Text == "O" && button4.Text == "O" && button7.Text == "O")
                || (button2.Text == "O" && button5.Text == "O" && button8.Text == "O")
                || (button3.Text == "O" && button6.Text == "O" && button9.Text == "O")
                || (button1.Text == "O" && button5.Text == "O" && button9.Text == "O")
                || (button3.Text == "O" && button5.Text == "O" && button7.Text == "O"))
            {
                MessageBox.Show("Player 2 Win", "Thông Báo");
                player2Wons++;
                lbPlayer2Win.Text = player2Wons.ToString();
                RestartGames();
            }
        }
        private void RestartGames()
        {
            buttons = new List<Button>() { button1, button2, button3, button4, button5, button6, button7, button8, button9 };
            btnMoves = new List<Button>();
            foreach (Button btn in buttons)
            {
                btn.Enabled = true;
                btn.Text = "?";
                btn.BackColor = Color.White;
            }
            timerLeft.Start();
        }
        private void PlayerRandomClick()
        {
            if (buttons.Count > 0)
            {
                int index = Utils.RandomNumber(0, buttons.Count);
                Button btn = buttons[index];
                if (btn.Text == "?")
                {
                    currPlayer = Player.X;
                    btn.Text = currPlayer.ToString();
                    btn.BackColor = Color.OrangeRed;
                    btn.Enabled = false;
                    buttons.Remove(btn);
                    btnMoves.Add(btn);
                    RemoveBtn();
                    timerLeft.Stop();
                    CheckGames();
                }
            }
        }
        private void RemoveBtn()
        {
            if (btnMoves != null && btnMoves.Count > 4)
            {
                if (btnDelete.Text == "")
                {
                    btnDelete = btnMoves[0];
                }
                if (isButtonVisible)
                {
                    btnDelete.BackColor = Color.LightPink;
                }
                else
                {
                    btnDelete.BackColor = Color.LightYellow;
                }
                isButtonVisible = !isButtonVisible;
                if (btnMoves.Count > 5)
                {
                    btnDelete.Enabled = true;
                    btnDelete.Text = "?";
                    btnDelete.BackColor = Color.White;
                    buttons.Add(btnDelete);
                    btnMoves.Remove(btnDelete);
                    btnDelete = new Button();
                }
            }
        }
        private void removeBtn(object sender, EventArgs e)
        {
            RemoveBtn();
        }
        private void timerLeft_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0)
            {
                timeLeft--;
                lbTime.Text = timeLeft.ToString();
            }
            else if (timeLeft <= 0)
            {
                PlayerRandomClick();
                timerLeft.Stop();
            }
        }
        private void ResetBtn_Click(object sender, EventArgs e)
        {
            RestartGames();
        }
        private void PlayerClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;

            if (btn.Text == "?")
            {
                if (currPlayer == Player.X)
                {
                    currPlayer = Player.O;
                    btn.BackColor = Color.Cyan;
                }
                else
                {
                    currPlayer = Player.X;
                    btn.BackColor = Color.OrangeRed;
                }
                btn.Text = currPlayer.ToString();
                btn.Enabled = false;
                btnMoves.Add(btn);
                buttons.Remove(btn);
                RemoveBtn();
                timeLeft = timeSet;
                CheckGames();
            }
        }
    }
}
