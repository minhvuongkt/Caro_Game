using Client.Constants;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Client.MainForm
{
    public partial class PlayBot : Form
    {
        // level 1 -> hard
        // level 0 -> easy
        Button btnDelete { get; set; } = new Button();
        private int levelBot { get; set; }
        private bool isButtonVisible { get; set; }
        int playerWons { get; set; } = 0;
        int botWons { get; set; } = 0;
        int timeLeft { get; set; } = 10;
        List<Button> buttons { get; set; }
        List<Button> btnMoves { get; set; }
        bool isAnotherMove { get; set; } = false;
        Player currPlayer { get; set; }
        public PlayBot(int level = 0)
        {
            levelBot = level;
            if (level == 1)
            {
                timeLeft = 3;
            }
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeComponent();
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
                botTimer.Stop();
                MessageBox.Show("Player Win", "Thông Báo");
                playerWons++;
                lbPlayerWin.Text = playerWons.ToString();
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
                botTimer.Stop();
                MessageBox.Show("Gà để bot ăn kìa", "Thông Báo");
                botWons++;
                lbBotWin.Text = botWons.ToString();
                RestartGames();
            }
        }
        private void RestartGames()
        {
            isAnotherMove = false;
            botTimer.Stop();
            buttons = new List<Button>() { button1, button2, button3, button4, button5, button6, button7, button8, button9 };
            btnMoves = new List<Button>();
            foreach (Button btn in buttons)
            {
                btn.Enabled = true;
                btn.Text = "?";
                btn.BackColor = Color.White;
            }
            if (levelBot == 1) timeLeft = 3;
            if (levelBot == 0) timeLeft = 10;
            timerLeft.Start();
        }
        public void BotMoveHard()
        {
            bool checkBtn1 = button1.Text == button3.Text || button1.Text == button5.Text || button1.Text == button7.Text
                || button1.Text == button2.Text || button1.Text == button9.Text;
            bool checkBtn2 = button2.Text == button3.Text || button2.Text == button8.Text || button2.Text == button5.Text;
            bool checkBtn3 = button3.Text == button9.Text || button3.Text == button6.Text || button3.Text == button5.Text || button3.Text == button7.Text;
            bool checkBtn4 = button4.Text == button6.Text || button4.Text == button5.Text || button4.Text == button7.Text;
            bool checkBtn5 = button5.Text == button9.Text || button5.Text == button6.Text || button5.Text == button8.Text || button5.Text == button7.Text;
            bool checkBtn6 = button6.Text == button9.Text;

            // Kiểm tra và chặn các trường hợp của Button 1
            if (button1.Text == "X")
            {
                if (button1.Text == button3.Text && button2.Text == "?")
                {
                    BotMove(button2);
                    return;
                }
                if (button1.Text == button5.Text && button9.Text == "?")
                {
                    BotMove(button9);
                    return;
                }
                if (button1.Text == button7.Text && button4.Text == "?")
                {
                    BotMove(button4);
                    return;
                }
                if (button1.Text == button2.Text && button3.Text == "?")
                {
                    BotMove(button3);
                    return;
                }
            }

            // Kiểm tra và chặn các trường hợp của Button 2
            if (button2.Text == "X")
            {
                if (button2.Text == button3.Text && button1.Text == "?")
                {
                    BotMove(button1);
                    return;
                }
                if (button2.Text == button5.Text && button8.Text == "?")
                {
                    BotMove(button8);
                    return;
                }
            }

            // Kiểm tra và chặn các trường hợp của Button 3
            if (button3.Text == "X")
            {
                if (button3.Text == button6.Text && button9.Text == "?")
                {
                    BotMove(button9);
                    return;
                }
                if (button3.Text == button9.Text && button6.Text == "?")
                {
                    BotMove(button6);
                    return;
                }
                if (button3.Text == button5.Text && button7.Text == "?")
                {
                    BotMove(button7);
                    return;
                }
            }

            // Kiểm tra và chặn các trường hợp của Button 4
            if (button4.Text == "X")
            {
                if (button4.Text == button5.Text && button6.Text == "?")
                {
                    BotMove(button6);
                    return;
                }
                if (button4.Text == button7.Text && button1.Text == "?")
                {
                    BotMove(button1);
                    return;
                }
            }

            // Kiểm tra và chặn các trường hợp của Button 5
            if (button5.Text == "X")
            {
                if (button5.Text == button6.Text && button4.Text == "?")
                {
                    BotMove(button4);
                    return;
                }
                if (button5.Text == button9.Text && button1.Text == "?")
                {
                    BotMove(button1);
                    return;
                }
                if (button5.Text == button8.Text && button2.Text == "?")
                {
                    BotMove(button2);
                    return;
                }
                if (button5.Text == button7.Text && button3.Text == "?")
                {
                    BotMove(button3);
                    return;
                }
            }

            // Kiểm tra và chặn các trường hợp của Button 6
            if (button6.Text == "X")
            {
                if (button6.Text == button9.Text && button3.Text == "?")
                {
                    BotMove(button3);
                    return;
                }
            }

            // Kiểm tra và chặn các trường hợp của Button 7
            if (button7.Text == "X")
            {
                if (button7.Text == button8.Text && button9.Text == "?")
                {
                    BotMove(button9);
                    return;
                }
                if (button7.Text == button9.Text && button8.Text == "?")
                {
                    BotMove(button8);
                    return;
                }
                if (button7.Text == button5.Text && button3.Text == "?")
                {
                    BotMove(button3);
                    return;
                }
            }

            // Kiểm tra và chặn các trường hợp của Button 8
            if (button8.Text == "X")
            {
                if (button8.Text == button9.Text && button7.Text == "?")
                {
                    BotMove(button7);
                    return;
                }
            }

            // Kiểm tra và chặn các trường hợp của Button 9
            if (button9.Text == "X")
            {
                if (button9.Text == button6.Text && button3.Text == "?")
                {
                    BotMove(button3);
                    return;
                }
                if (button9.Text == button8.Text && button7.Text == "?")
                {
                    BotMove(button7);
                    return;
                }
            }
            if (buttons.Count > 0 && isAnotherMove && !(checkBtn1 && checkBtn2 && checkBtn3 && checkBtn4 && checkBtn5 && checkBtn6))
            {
                BotMoveEasy();
                return;
            }
        }
        public void BotMove(Button btn)
        {
            if (btn.Text == "?")
            {
                currPlayer = Player.O;
                btn.Text = currPlayer.ToString();
                btn.BackColor = Color.Cyan;
                btn.Enabled = false;
                buttons.Remove(btn);
                btnMoves.Add(btn);
                isAnotherMove = false;
                botTimer.Stop();
                RemoveBtn();
                if (levelBot == 1) timeLeft = 3;
                if (levelBot == 0) timeLeft = 10;
                timerLeft.Start();
                CheckGames();
            }
        }
        public void BotMoveEasy()
        {
            if (buttons.Count > 0 && isAnotherMove)
            {
                int index = Utils.RandomNumber(0, buttons.Count);
                Button btn = buttons[index];
                BotMove(btn);
            }
        }
        private void botTimer_Tick(object sender, EventArgs e)
        {
            if (levelBot == 0)
            {
                BotMoveEasy();
            }
            else if (levelBot == 1)
            {
                BotMoveHard();
            }
        }
        private void ResetBtn_Click(object sender, EventArgs e)
        {
            RestartGames();
        }
        private void PlayerClick(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            if (btn.Text == "?" && isAnotherMove == false)
            {
                currPlayer = Player.X;
                btn.BackColor = Color.OrangeRed;
                btn.Text = currPlayer.ToString();
                btn.Enabled = false;
                buttons.Remove(btn);
                btnMoves.Add(btn);
                isAnotherMove = true;
                RemoveBtn();
                timerLeft.Stop();
                botTimer.Start();
                CheckGames();
            }
        }
        private void PlayerRandomClick()
        {
            if (buttons.Count > 0 && !isAnotherMove)
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
                    isAnotherMove = true;
                    RemoveBtn();
                    timerLeft.Stop();
                    botTimer.Start();
                    CheckGames();
                }
            }
        }
        private void RemoveBtn()
        {
            var maxCount = 4;
            var maxCountDel = 5;
            /*if (levelBot == 1)
            {
                maxCount = 3;
                maxCountDel = 4;
            }*/
            if (btnMoves != null && btnMoves.Count > maxCount)
            {
                //var btn = new Button();
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
                if (btnMoves.Count > maxCountDel)
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
            else if (timeLeft <= 0 && isAnotherMove == false)
            {
                PlayerRandomClick();
                timerLeft.Stop();
            }
        }
    }
}
