using Client.Connection;
using Client.Constants;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Client.MainForm
{
    public partial class RoomLobby : Form
    {
        private System.Windows.Forms.Timer _refreshTimer;
        private bool _isQueued; // tracks whether we are in the random-match queue

        public RoomLobby()
        {
            InitializeComponent();
            UITheme.Apply(this);

            // Subscribe to room list updates from server
            GameClient.Instance.OnRoomListReceived += OnRoomListReceived;
            GameClient.Instance.OnRoomJoined       += OnRoomJoined;
            GameClient.Instance.OnGameStateUpdated += OnGameStateReceived;
            GameClient.Instance.OnRoomInviteReceived += OnInviteReceived;
            // Matchmaking events
            GameClient.Instance.OnMatchQueued    += OnMatchQueued;
            GameClient.Instance.OnMatchFound     += OnMatchFound;
            GameClient.Instance.OnMatchConfirmed += OnMatchConfirmed;
            GameClient.Instance.OnMatchCancelled += OnMatchCancelled;

            // Auto-refresh every 3 s
            _refreshTimer = new System.Windows.Forms.Timer { Interval = 3000 };
            _refreshTimer.Tick += (s, e) => GameClient.Instance.RequestRoomList();
            _refreshTimer.Start();

            GameClient.Instance.RequestRoomList();
        }

        // ── Server callbacks ─────────────────────────────────────────────────

        private void OnRoomListReceived(List<Room> rooms)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnRoomListReceived(rooms))); return; }
            RefreshRoomList(rooms);
        }

        private void OnRoomJoined(Room room)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnRoomJoined(room))); return; }
            DataCache.IsSpectating = false;
            OpenOnlineGame(room);
        }

        private void OnGameStateReceived(Room room)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnGameStateReceived(room))); return; }
            // GameState arrives when joining as spectator; open game window in spectator mode
            DataCache.IsSpectating = true;
            OpenOnlineGame(room);
        }

        private void OnInviteReceived(RoomInviteData invite)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnInviteReceived(invite))); return; }
            var result = MessageBox.Show(
                $"{invite.FromUID} đã mời bạn vào phòng \"{invite.RoomName}\" ({invite.GameType}).\nChấp nhận?",
                "Lời mời", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                GameClient.Instance.AcceptInvite(invite.RoomID);
            else
                GameClient.Instance.DeclineInvite(invite.RoomID);
        }

        private void OpenOnlineGame(Room room)
        {
            _refreshTimer.Stop();
            GameClient.Instance.OnRoomListReceived   -= OnRoomListReceived;
            GameClient.Instance.OnRoomJoined         -= OnRoomJoined;
            GameClient.Instance.OnGameStateUpdated   -= OnGameStateReceived;
            GameClient.Instance.OnRoomInviteReceived -= OnInviteReceived;
            GameClient.Instance.OnMatchQueued        -= OnMatchQueued;
            GameClient.Instance.OnMatchFound         -= OnMatchFound;
            GameClient.Instance.OnMatchConfirmed     -= OnMatchConfirmed;
            GameClient.Instance.OnMatchCancelled     -= OnMatchCancelled;

            var game = new OnlineGame(room);
            game.Show();
            this.Hide();
        }

        // ── UI helpers ───────────────────────────────────────────────────────

        private void RefreshRoomList(List<Room> rooms)
        {
            lvRooms.Items.Clear();
            foreach (var r in rooms)
            {
                int players = CountPlayers(r);
                int maxPlayers = r.GameType == GameType.TwoVsTwo ? 4 : 2;
                var item = new ListViewItem(r.Name);
                item.SubItems.Add(r.GameType.ToString());
                item.SubItems.Add(r.Status.ToString());
                item.SubItems.Add($"{players}/{maxPlayers}");
                item.SubItems.Add(r.SpectatorUIDs?.Count.ToString() ?? "0");
                item.Tag = r.RoomID;
                lvRooms.Items.Add(item);
            }
        }

        private static int CountPlayers(Room r)
        {
            int c = 0;
            if (!string.IsNullOrEmpty(r.Player1UID)) c++;
            if (!string.IsNullOrEmpty(r.Player2UID)) c++;
            if (!string.IsNullOrEmpty(r.Team1AllyUID)) c++;
            if (!string.IsNullOrEmpty(r.Team2AllyUID)) c++;
            return c;
        }

        // ── Button handlers ──────────────────────────────────────────────────

        private void btnCreate_Click(object sender, EventArgs e)
        {
            using (var dlg = new CreateRoomDialog())
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                    GameClient.Instance.CreateRoom(dlg.RoomName, dlg.GameType);
            }
        }

        private void btnJoinPlayer_Click(object sender, EventArgs e)
        {
            string roomId = GetSelectedRoomId();
            if (roomId == null) return;
            GameClient.Instance.JoinRoom(roomId);
        }

        private void btnSpectate_Click(object sender, EventArgs e)
        {
            string roomId = GetSelectedRoomId();
            if (roomId == null) return;
            GameClient.Instance.SpectateRoom(roomId);
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            GameClient.Instance.RequestRoomList();
        }

        private void btnGroupChat_Click(object sender, EventArgs e)
        {
            var chat = new ChatForm();
            chat.Show();
        }

        private void btnMatchQueue_Click(object sender, EventArgs e)
        {
            if (_isQueued)
            {
                GameClient.Instance.CancelMatchQueue();
                _isQueued = false;
                btnMatchQueue.Text = "Ghép trận ngẫu nhiên";
            }
            else
            {
                GameClient.Instance.QueueForMatch("Caro");
                _isQueued = true;
                btnMatchQueue.Text = "⏳ Đang tìm đối thủ… (bấm để hủy)";
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // ── Matchmaking callbacks ────────────────────────────────────────────

        private void OnMatchQueued(string gameType)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnMatchQueued(gameType))); return; }
            _isQueued = true;
            btnMatchQueue.Text = $"⏳ Đang tìm đối thủ ({gameType})… (bấm để hủy)";
        }

        private void OnMatchFound(Client.Models.MatchFoundData data)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnMatchFound(data))); return; }
            _isQueued = false;
            btnMatchQueue.Text = "Ghép trận ngẫu nhiên";
            var result = MessageBox.Show(
                $"Đã tìm thấy đối thủ!\n\nĐối thủ: {data.OpponentName}\nGame: {data.GameType}\n\nBạn có muốn chấp nhận trận đấu này không?",
                "Ghép trận ngẫu nhiên", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                GameClient.Instance.AcceptMatch(data.MatchID);
            else
                GameClient.Instance.DeclineMatch(data.MatchID);
        }

        private void OnMatchConfirmed(Client.Models.Room room)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnMatchConfirmed(room))); return; }
            DataCache.IsSpectating = false;
            OpenOnlineGame(room);
        }

        private void OnMatchCancelled(string reason)
        {
            if (InvokeRequired) { Invoke(new Action(() => OnMatchCancelled(reason))); return; }
            _isQueued = false;
            btnMatchQueue.Text = "Ghép trận ngẫu nhiên";
            MessageBox.Show(reason, "Ghép trận", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RoomLobby_FormClosing(object sender, FormClosingEventArgs e)
        {
            _refreshTimer.Stop();
            GameClient.Instance.OnRoomListReceived   -= OnRoomListReceived;
            GameClient.Instance.OnRoomJoined         -= OnRoomJoined;
            GameClient.Instance.OnGameStateUpdated   -= OnGameStateReceived;
            GameClient.Instance.OnRoomInviteReceived -= OnInviteReceived;
            GameClient.Instance.OnMatchQueued        -= OnMatchQueued;
            GameClient.Instance.OnMatchFound         -= OnMatchFound;
            GameClient.Instance.OnMatchConfirmed     -= OnMatchConfirmed;
            GameClient.Instance.OnMatchCancelled     -= OnMatchCancelled;
            var caroGames = new CaroGames();
            caroGames.Show();
        }

        private string GetSelectedRoomId()
        {
            if (lvRooms.SelectedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một phòng.", "Thông báo");
                return null;
            }
            return lvRooms.SelectedItems[0].Tag as string;
        }
    }

    // ── Inline "Create Room" dialog ──────────────────────────────────────────

    internal class CreateRoomDialog : Form
    {
        public string RoomName { get; private set; }
        public string GameType { get; private set; }

        private TextBox txtName;
        private ComboBox cmbType;
        private Button btnOk, btnCancel;

        public CreateRoomDialog()
        {
            Text = "Tạo phòng mới";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new System.Drawing.Size(320, 150);
            MaximizeBox = false; MinimizeBox = false;

            var lbl1 = new Label { Text = "Tên phòng:", Location = new System.Drawing.Point(12, 15), AutoSize = true };
            txtName = new TextBox { Location = new System.Drawing.Point(110, 12), Width = 190 };

            var lbl2 = new Label { Text = "Loại game:", Location = new System.Drawing.Point(12, 50), AutoSize = true };
            cmbType = new ComboBox { Location = new System.Drawing.Point(110, 47), Width = 190, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new object[] { "Caro (15x15)", "Go (19x19)", "2v2 Caro (15x15)" });
            cmbType.SelectedIndex = 0;

            btnOk = new Button { Text = "Tạo", DialogResult = DialogResult.OK, Location = new System.Drawing.Point(140, 100), Width = 80 };
            btnOk.Click += (s, e) =>
            {
                RoomName = txtName.Text.Trim();
                if (string.IsNullOrEmpty(RoomName)) { MessageBox.Show("Nhập tên phòng."); DialogResult = DialogResult.None; return; }
                GameType = cmbType.SelectedIndex == 1 ? "Go" : cmbType.SelectedIndex == 2 ? "TwoVsTwo" : "Caro";
            };
            btnCancel = new Button { Text = "Hủy", DialogResult = DialogResult.Cancel, Location = new System.Drawing.Point(230, 100), Width = 80 };

            Controls.AddRange(new Control[] { lbl1, txtName, lbl2, cmbType, btnOk, btnCancel });
            AcceptButton = btnOk; CancelButton = btnCancel;

            UITheme.Apply(this);
        }
    }
}
