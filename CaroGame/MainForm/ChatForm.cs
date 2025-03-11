using Client.Connection;
using Client.Constants;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client.MainForm
{    
    public partial class ChatForm : Form
    {
        readonly ChatClient _chatClient;
        private string currentFriendUID { get; set; }
        private PlayerClient plHandler { get; set; }


        public ChatForm()
        {
            _chatClient = new ChatClient();
            InitializeComponent();
            _chatClient.OnMessageReceived += OnMessageReceivedHandler;
            // Bắt đầu luồng nhận tin nhắn
            Task.Run(() => _chatClient.ReceiveMessages());
            DisableControls();
            LoadFriends();
        }
        private void DisableControls()
        {
            if (listBoxFriend.Items.Count < 1 || currentFriendUID == "")
            {
                txtMessage.Visible = false;
                txtChatWith.Visible = false;
                btnSend.Visible = false;
                richTextBoxMessages.Visible = false;
            }
            else
            {
                txtMessage.Visible = true;
                txtChatWith.Visible = true;
                btnSend.Visible = true;
                richTextBoxMessages.Visible = true;
            }
        }
        private void LoadFriends()
        {
            listBoxFriend.Items.Clear();

            // Thêm chat nhóm vào đầu danh sách
            listBoxFriend.Items.Add("Group Chat");

            // Thêm danh sách bạn bè vào listBoxFriend
            foreach (var friend in DataCache.Player.Friends)
            {
                listBoxFriend.Items.Add(friend.UID + "|" + friend.Name); 
            }

            // Nếu có bạn bè được chọn trước đó, tải lại tin nhắn từ cache
            if (!string.IsNullOrEmpty(currentFriendUID))
            {
                LoadChatFromCache(currentFriendUID);
            }
        }
        private void listBoxFriend_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBoxFriend.SelectedItem == null) return;
            if(listBoxFriend.SelectedItem.ToString() == "Group Chat") currentFriendUID = listBoxFriend.SelectedItem.ToString(); //Uid group chat
            else currentFriendUID = listBoxFriend.SelectedItem.ToString().Split('|')[0]; //Lấy uid người dùng
            DisableControls();
            // Xóa nội dung hiện tại
            richTextBoxMessages.Clear();

            if (currentFriendUID == "Group Chat")
            {
                txtChatWith.Text = "Group Chat";
                // Yêu cầu tin nhắn mới từ server nếu chưa có trong cache
                if (DataCache.groupChatCache.Count <= 0)
                {
                    RequestNewMessages(currentFriendUID, DateTime.MinValue);
                }
                // Hiển thị tin nhắn từ cache của group chat
                foreach (var message in DataCache.groupChatCache)
                {
                    UpdateChatUI(message);
                }
            }
            else
            {
                txtChatWith.Text = GetNameByUID(currentFriendUID);
                // Hiển thị tin nhắn từ cache của bạn bè
                LoadChatFromCache(currentFriendUID);

                // Yêu cầu tin nhắn mới từ server nếu chưa có trong cache
                if (!DataCache.chatCache.ContainsKey(currentFriendUID))
                {
                    RequestNewMessages(currentFriendUID, DateTime.MinValue);
                }
                else
                {
                    var lastMessageTime = DataCache.chatCache[currentFriendUID].Last().Time;
                    RequestNewMessages(currentFriendUID, lastMessageTime);
                }
            }
        }

        private void LoadChatFromCache(string friendUID)
        {
            if (DataCache.chatCache.ContainsKey(friendUID))
            {
                var cachedMessages = DataCache.chatCache[friendUID];
                foreach (var message in cachedMessages)
                {
                    UpdateChatUI(message);
                }
            }
        }

        private void RequestNewMessages(string friendUID, DateTime lastMessageTime)
        {
            var request = new MessageRequest
            {
                FriendUID = friendUID,
                LastMessageTime = lastMessageTime
            };
            var msg = new Models.Message()
            {
                MessageType = "MessageRequest",
                Data = request
            };
            // Gửi yêu cầu tới server để nhận tin nhắn mới
            _chatClient._connectToServer.Send(msg);
        }
        private void OnMessageReceivedHandler(Chat chat)
        {
            Invoke(new Action(() =>
            {
                if (chat.IsGroupChat)
                {
                    // Cập nhật cache cho group chat
                    DataCache.groupChatCache.Add(chat);

                    // Nếu người dùng đang ở màn hình group chat, cập nhật giao diện
                    if (txtChatWith.Text == "Group Chat")
                    {
                        UpdateChatUI(chat);
                    }
                }
                else
                {
                    // Cập nhật cache cho chat cá nhân
                    if (!DataCache.chatCache.ContainsKey(chat.ReceiverUID))
                    {
                        DataCache.chatCache[chat.ReceiverUID] = new List<Chat>();
                    }
                    DataCache.chatCache[chat.ReceiverUID].Add(chat);

                    // Nếu người dùng đang ở màn hình của người bạn đó, cập nhật giao diện
                    if (chat.ReceiverUID == currentFriendUID)
                    {
                        UpdateChatUI(chat);
                    }
                }
            }));
        }

        private void UpdateChatUI(Chat chat)
        {
            string displayMessage = chat.Message;

            if (chat.IsGroupChat)
            {
                txtChatWith.Text = "Group Chat";
                if (chat.SenderUID == DataCache.Player.UID)
                {
                    richTextBoxMessages.SelectionAlignment = HorizontalAlignment.Right;
                    richTextBoxMessages.AppendText($"{displayMessage} :[You] \n");
                }
                else
                {
                    txtChatWith.Text = $"{GetNameByUID(chat.ReceiverUID)}";
                    richTextBoxMessages.SelectionAlignment = HorizontalAlignment.Left;
                    richTextBoxMessages.AppendText($"{GetNameByUID(chat.SenderUID)}: {displayMessage} \n");
                }
                /*
                richTextBoxMessages.SelectionAlignment = chat.SenderUID == DataCache.Player.UID ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                richTextBoxMessages.AppendText(chat.SenderUID == DataCache.Player.UID ? "[You]" : $"[{GetNameByUID(chat.SenderUID)}]"+$"  {displayMessage}\n");*/
            }
            else
            {
                // Phân biệt tin nhắn của người dùng hiện tại với tin nhắn của người khác
                if (chat.SenderUID == DataCache.Player.UID)
                {
                    richTextBoxMessages.SelectionAlignment = HorizontalAlignment.Right;
                    richTextBoxMessages.AppendText($"{displayMessage} :[You] \n");
                }
                else
                {
                    txtChatWith.Text = $"{GetNameByUID(chat.ReceiverUID)}";
                    richTextBoxMessages.SelectionAlignment = HorizontalAlignment.Left;
                    richTextBoxMessages.AppendText($"{GetNameByUID(chat.SenderUID)}: {displayMessage} \n");
                }
            }

            // Cuộn xuống phần tử cuối cùng
            richTextBoxMessages.SelectionStart = richTextBoxMessages.Text.Length;
            richTextBoxMessages.ScrollToCaret();
        }


        private void btnAddUID_Click(object sender, EventArgs e)
        {
            string uid = txtUID.Text.Trim();

            if (!string.IsNullOrEmpty(uid))
            {
                if (!listBoxFriend.Items.Contains("Group Chat"))
                {
                    listBoxFriend.Items.Insert(0, "Group Chat");
                }
                else
                {
                    // Kiểm tra xem UID đã có trong danh sách bạn bè chưa
                    if (!listBoxFriend.Items.Contains(uid))
                    {
                        listBoxFriend.Items.Add(uid);
                        // Thêm bạn vào danh sách bạn bè của người dùng hiện tại
                        DataCache.Player.Friends.Add(new Friend { UID = uid, Name = GetNameByUID(uid) });
                    }
                }
                plHandler.UpdatePlayer(DataCache.Player);
                // Xóa nội dung của txtUID
                txtUID.Clear();
            }
        }

        private string GetNameByUID(string uid)
        {
            // Thay thế phần này với cách bạn lấy tên thực tế từ UID
            return DataCache.Player.Friends.FirstOrDefault(f => f.UID == uid)?.Name ?? uid;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string message = txtMessage.Text.Trim();
            string recipientUID = currentFriendUID;

            if (!string.IsNullOrEmpty(message))
            {
                var chatMsg = new Chat()
                {
                    SenderUID = DataCache.Player.UID,
                    ReceiverUID = recipientUID,
                    Message = message,
                    Time = DateTime.Now,
                    IsGroupChat = recipientUID == "Group Chat"  // Nếu là group chat
                };

                if (chatMsg.IsGroupChat)
                {
                    // Lưu tin nhắn vào cache group chat
                    DataCache.groupChatCache.Add(chatMsg);
                }
                else
                {
                    // Lưu tin nhắn vào cache cho người bạn
                    if (!DataCache.chatCache.ContainsKey(recipientUID))
                    {
                        DataCache.chatCache[recipientUID] = new List<Chat>();
                    }
                    DataCache.chatCache[recipientUID].Add(chatMsg);
                }

                // Gửi tin nhắn đến server
                _chatClient.SendMessage(chatMsg);

                // Cập nhật giao diện
                UpdateChatUI(chatMsg);

                // Xóa nội dung của txtMessage
                txtMessage.Clear();
            }
        }
    }

}
