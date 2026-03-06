using Client.Connection;
using Client.Constants;
using Client.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Client.MainForm
{
    /// <summary>
    /// Online game board.  Works for Caro (15x15), Go (19x19) and 2v2 Caro (15x15).
    /// Spectators see the board but cannot click or chat.
    /// </summary>
    public class OnlineGame : Form
    {
        // ── Layout constants ─────────────────────────────────────────────────
        private const int CELL_SIZE     = 36;   // px per cell
        private const int BOARD_PADDING = 20;   // px margin inside board panel
        private const int CHAT_WIDTH    = 220;
        private const int STAR_R        = 4;    // radius of Go star-point dot

        // ── Controls ─────────────────────────────────────────────────────────
        private Panel      pnlBoard;
        private RichTextBox rtbChat;
        private TextBox    txtChat;
        private Button     btnSend;
        private Label      lblStatus;
        private Label      lblRoom;
        private Label      lblMyPiece;
        private Button     btnLeave;
        private Button     btnInvite;

        // ── State ─────────────────────────────────────────────────────────────
        private Room   _room;
        private bool   _isSpectating;
        private string _myUID;
        private string _myPiece;   // "X" or "O"

        // ── Constructor ───────────────────────────────────────────────────────
        public OnlineGame(Room room)
        {
            _room        = room;
            _isSpectating = DataCache.IsSpectating;
            _myUID       = DataCache.Player?.UID ?? "";
            _myPiece     = GetMyPiece(room, _myUID);

            BuildUI();

            GameClient.Instance.OnGameStateUpdated  += OnGameStateUpdated;
            GameClient.Instance.OnRoomChatReceived  += OnRoomChatReceived;

            UpdateStatus();
        }

        // ── UI construction ───────────────────────────────────────────────────
        private void BuildUI()
        {
            int boardPx = _room.BoardSize * CELL_SIZE + BOARD_PADDING * 2;
            int formW   = boardPx + CHAT_WIDTH + 20;
            int formH   = boardPx + 100;

            this.Text            = $"Phòng: {_room.Name}  ({_room.GameType})";
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox     = false;
            this.ClientSize      = new Size(formW, formH);
            this.StartPosition   = FormStartPosition.CenterScreen;
            this.BackColor       = UITheme.BgDark;
            this.ForeColor       = UITheme.TextPrimary;
            this.FormClosing    += OnFormClosing;

            // Board panel
            pnlBoard = new Panel
            {
                Location  = new Point(10, 50),
                Size      = new Size(boardPx, boardPx),
                BackColor = UITheme.BgPanel
            };
            pnlBoard.Paint      += OnBoardPaint;
            pnlBoard.MouseClick += OnBoardClick;

            // Status labels
            lblRoom = new Label
            {
                Location  = new Point(10, 10),
                Size      = new Size(boardPx, 30),
                Font      = new Font("Segoe UI Semibold", 10f),
                ForeColor = UITheme.TextAccent,
                BackColor = Color.Transparent,
                Text      = $"Phòng: {_room.Name} | Game: {_room.GameType}"
            };
            lblStatus = new Label
            {
                Location  = new Point(10, formH - 40),
                Size      = new Size(boardPx, 26),
                Font      = new Font("Segoe UI", 9.5f),
                ForeColor = UITheme.TextPrimary,
                BackColor = Color.Transparent
            };
            lblMyPiece = new Label
            {
                Location  = new Point(boardPx + 20, 50),
                Size      = new Size(CHAT_WIDTH - 10, 24),
                Font      = new Font("Segoe UI Semibold", 9.5f),
                ForeColor = UITheme.AccentGreen,
                BackColor = Color.Transparent
            };

            // Chat area
            rtbChat = new RichTextBox
            {
                Location    = new Point(boardPx + 20, 80),
                Size        = new Size(CHAT_WIDTH - 10, formH - 180),
                ReadOnly    = true,
                BackColor   = UITheme.InputBg,
                ForeColor   = UITheme.TextPrimary,
                BorderStyle = BorderStyle.None
            };
            txtChat = new TextBox
            {
                Location  = new Point(boardPx + 20, formH - 90),
                Size      = new Size(CHAT_WIDTH - 75, 28),
                BackColor = UITheme.InputBg,
                ForeColor = UITheme.TextPrimary,
                BorderStyle = BorderStyle.FixedSingle,
                Enabled   = !_isSpectating
            };
            txtChat.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) SendChat(); };

            btnSend = new Button
            {
                Text      = "Gửi",
                Location  = new Point(boardPx + CHAT_WIDTH - 47, formH - 90),
                Size      = new Size(47, 28),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.AccentBlue,
                ForeColor = UITheme.TextPrimary,
                Font      = UITheme.SmallFont,
                Cursor    = Cursors.Hand,
                Enabled   = !_isSpectating
            };
            btnSend.FlatAppearance.BorderSize = 0;
            btnSend.Click += (s, e) => SendChat();

            btnLeave = new Button
            {
                Text      = "Rời phòng",
                Location  = new Point(boardPx + 20, formH - 50),
                Size      = new Size(100, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.AccentRed,
                ForeColor = UITheme.TextPrimary,
                Font      = UITheme.SmallFont,
                Cursor    = Cursors.Hand
            };
            btnLeave.FlatAppearance.BorderSize = 0;
            btnLeave.Click += (s, e) => this.Close();

            btnInvite = new Button
            {
                Text      = "Mời bạn",
                Location  = new Point(boardPx + 130, formH - 50),
                Size      = new Size(90, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.AccentBlue,
                ForeColor = UITheme.TextPrimary,
                Font      = UITheme.SmallFont,
                Cursor    = Cursors.Hand,
                Enabled   = !_isSpectating
            };
            btnInvite.FlatAppearance.BorderSize = 0;
            btnInvite.Click += OnInviteClick;

            this.Controls.AddRange(new Control[] {
                lblRoom, lblStatus, pnlBoard,
                lblMyPiece, rtbChat, txtChat, btnSend, btnLeave, btnInvite });

            // Show spectator notice
            if (_isSpectating)
                AppendChat("[Hệ thống]", "Bạn đang xem – không thể nhắn tin hay đánh cờ.");

            // Show which piece you play
            if (!_isSpectating && !string.IsNullOrEmpty(_myPiece))
            {
                lblMyPiece.Text = $"Quân của bạn: {PieceDisplayName(_myPiece)}";
            }
            else if (_isSpectating)
            {
                lblMyPiece.Text = "Chế độ: Xem";
            }
        }

        // ── Board painting ────────────────────────────────────────────────────
        private void OnBoardPaint(object sender, PaintEventArgs e)
        {
            if (_room?.Board == null) return;
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int n    = _room.BoardSize;
            bool isGo = _room.GameType == GameType.Go;

            // Grid lines
            using (var pen = new Pen(UITheme.BorderColor))
            {
                for (int i = 0; i < n; i++)
                {
                    int x = BOARD_PADDING + i * CELL_SIZE;
                    int y = BOARD_PADDING + i * CELL_SIZE;
                    g.DrawLine(pen, x, BOARD_PADDING, x, BOARD_PADDING + (n - 1) * CELL_SIZE);
                    g.DrawLine(pen, BOARD_PADDING, y, BOARD_PADDING + (n - 1) * CELL_SIZE, y);
                }
            }

            // Star points for Go (19x19)
            if (isGo && n == 19)
            {
                int[] stars = { 3, 9, 15 };
                foreach (var r in stars)
                    foreach (var c in stars)
                    {
                        int cx = BOARD_PADDING + c * CELL_SIZE;
                        int cy = BOARD_PADDING + r * CELL_SIZE;
                        g.FillEllipse(Brushes.Gray, cx - STAR_R, cy - STAR_R, STAR_R * 2, STAR_R * 2);
                    }
            }

            // Pieces
            for (int row = 0; row < n; row++)
            {
                for (int col = 0; col < n; col++)
                {
                    string piece = _room.Board[row * n + col];
                    if (string.IsNullOrEmpty(piece)) continue;

                    int cx = BOARD_PADDING + col * CELL_SIZE;
                    int cy = BOARD_PADDING + row * CELL_SIZE;

                    if (isGo)
                    {
                        // Draw filled circle at intersection
                        int r = CELL_SIZE / 2 - 3;
                        var brush = piece == "X" ? Brushes.Black : Brushes.White;
                        g.FillEllipse(brush, cx - r, cy - r, r * 2, r * 2);
                        g.DrawEllipse(Pens.Black, cx - r, cy - r, r * 2, r * 2);
                    }
                    else
                    {
                        // Caro: draw X or O in the cell centre
                        int cx2 = BOARD_PADDING + col * CELL_SIZE + CELL_SIZE / 2;
                        int cy2 = BOARD_PADDING + row * CELL_SIZE + CELL_SIZE / 2;
                        using (var font = new Font("Arial", CELL_SIZE * 0.45f, FontStyle.Bold))
                        using (var brush = new SolidBrush(PieceColor(piece)))
                        {
                            var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            g.DrawString(piece, font, brush, cx2, cy2, sf);
                        }
                    }
                }
            }

            // Highlight last move
            // (optional: track last move index and draw a red dot)
        }

        // ── Board click ───────────────────────────────────────────────────────
        private void OnBoardClick(object sender, MouseEventArgs e)
        {
            if (_isSpectating || _room == null || _room.Status != RoomStatus.Playing) return;
            if (_room.CurrentTurnUID != _myUID) return;

            int col = (e.X - BOARD_PADDING + CELL_SIZE / 2) / CELL_SIZE;
            int row = (e.Y - BOARD_PADDING + CELL_SIZE / 2) / CELL_SIZE;

            // For Go, snap to nearest intersection
            if (_room.GameType == GameType.Go)
            {
                col = (int)Math.Round((double)(e.X - BOARD_PADDING) / CELL_SIZE);
                row = (int)Math.Round((double)(e.Y - BOARD_PADDING) / CELL_SIZE);
            }

            if (row < 0 || row >= _room.BoardSize || col < 0 || col >= _room.BoardSize) return;
            if (!string.IsNullOrEmpty(_room.Board[row * _room.BoardSize + col])) return;

            GameClient.Instance.SendGameMove(_room.RoomID, row, col);
        }

        // ── Server event handlers ─────────────────────────────────────────────
        private void OnGameStateUpdated(Room room)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnGameStateUpdated(room))); return; }
            _room = room;
            _myPiece = GetMyPiece(room, _myUID);
            pnlBoard.Invalidate();
            UpdateStatus();

            if (room.Status == RoomStatus.Finished)
            {
                string msg;
                if (room.WinnerUID == "draw") msg = "Hòa!";
                else if (room.WinnerUID == _myUID) msg = "Bạn thắng! 🎉";
                else msg = $"Người chơi {room.WinnerUID} thắng.";
                MessageBox.Show(msg, "Kết quả", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void OnRoomChatReceived(Chat chat)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnRoomChatReceived(chat))); return; }
            string sender = chat.SenderUID == _myUID ? "Bạn" : chat.SenderUID;
            AppendChat(sender, chat.Message);
        }

        // ── Chat helpers ──────────────────────────────────────────────────────
        private void SendChat()
        {
            string msg = txtChat.Text.Trim();
            if (string.IsNullOrEmpty(msg) || _room == null) return;
            GameClient.Instance.SendRoomChat(_room.RoomID, msg);
            txtChat.Clear();
        }

        private void AppendChat(string sender, string message)
        {
            rtbChat.AppendText($"[{sender}]: {message}\n");
            rtbChat.ScrollToCaret();
        }

        // ── Invite button ─────────────────────────────────────────────────────
        private void OnInviteClick(object sender, EventArgs e)
        {
            string uid = Microsoft.VisualBasic.Interaction.InputBox("Nhập UID (IP) người chơi muốn mời:", "Mời bạn", "");
            if (!string.IsNullOrEmpty(uid))
                GameClient.Instance.InvitePlayer(uid);
        }

        // ── Status label ──────────────────────────────────────────────────────
        private void UpdateStatus()
        {
            if (_room == null) return;
            switch (_room.Status)
            {
                case RoomStatus.Waiting:
                    lblStatus.Text = "Đang chờ người chơi...";
                    break;
                case RoomStatus.Playing:
                    string turnName = _room.CurrentTurnUID == _myUID ? "Lượt của bạn!" : $"Lượt của {_room.CurrentTurnUID}";
                    lblStatus.Text = turnName;
                    lblStatus.ForeColor = _room.CurrentTurnUID == _myUID ? UITheme.AccentGreen : UITheme.TextMuted;
                    break;
                case RoomStatus.Finished:
                    lblStatus.Text = string.IsNullOrEmpty(_room.WinnerUID) ? "Ván đã kết thúc." :
                                     _room.WinnerUID == "draw" ? "Ván hòa!" : $"Người thắng: {_room.WinnerUID}";
                    lblStatus.ForeColor = UITheme.AccentAmber;
                    break;
            }

            if (!_isSpectating && !string.IsNullOrEmpty(_myPiece))
            {
                lblMyPiece.Text = $"Quân: {PieceDisplayName(_myPiece)}";
            }
        }

        // ── Closing ───────────────────────────────────────────────────────────
        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            GameClient.Instance.OnGameStateUpdated -= OnGameStateUpdated;
            GameClient.Instance.OnRoomChatReceived -= OnRoomChatReceived;
            GameClient.Instance.LeaveRoom();
            DataCache.CurrentRoom = null;
            DataCache.IsSpectating = false;

            var lobby = new RoomLobby();
            lobby.Show();
        }

        // ── Util ──────────────────────────────────────────────────────────────
        private static string GetMyPiece(Room room, string myUID)
        {
            if (myUID == room?.Player1UID || myUID == room?.Team1AllyUID) return "X";
            if (myUID == room?.Player2UID || myUID == room?.Team2AllyUID) return "O";
            return "";
        }

        /// <summary>Returns the display name for a piece ("Đen/X" or "Trắng/O").</summary>
        private static string PieceDisplayName(string piece)
            => piece == "X" ? "Đen/X" : "Trắng/O";

        /// <summary>Returns the draw color for a piece.</summary>
        private static Color PieceColor(string piece)
            => piece == "X" ? UITheme.CellX : UITheme.CellO;
    }
}
