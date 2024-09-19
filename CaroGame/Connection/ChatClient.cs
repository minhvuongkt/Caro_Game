using Client.Connection;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace Client.Constants
{
    public class ChatClient
    {
        public readonly ConnectToServer _connectToServer;
        public event Action<Chat> OnMessageReceived;
        public ChatClient()
        {
            _connectToServer = new ConnectToServer();
        }

        public void ReceiveMessages()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[1024 * 5000];
                    int received = _connectToServer.client.Receive(data);

                    if (received > 0)
                    {
                        byte[] validData = new byte[received];
                        Array.Copy(data, validData, received);
                        var receivedObj = _connectToServer.Deserialize<Models.Message>(validData);
                        if (receivedObj != null && !Utils._checkEmpty(receivedObj.MessageType))
                        {
                            if (receivedObj.MessageType == "ChatList" || receivedObj.MessageType == "Chat")
                            {
                                var chat = DeserializeFromJsonElement<Chat>(receivedObj.Data);
                                if (chat != null)
                                {
                                    // Gọi sự kiện để cập nhật giao diện chat
                                    OnMessageReceived?.Invoke(chat);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi, có thể ghi log hoặc thông báo cho người dùng
                    MessageBox.Show($"ReceiveMessages error: {ex.Message}");
                    break; // Ngắt vòng lặp khi có lỗi để tránh lỗi không hồi phục
                }
            }
        }
        private T DeserializeFromJsonElement<T>(object data)
        {
            if (data is JsonElement jsonElement)
            {
                // Convert JsonElement to the specific object
                return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
            }
            throw new InvalidCastException("Cannot cast the object to the specified type.");
        }
        public void SendMessage(Chat chat)
        {
            if (chat != null)
            {
                try
                {
                    var msg = new Models.Message()
                    {
                        MessageType = "Chat",
                        Data = chat
                    };
                    _connectToServer.Send(msg);
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi, có thể ghi log hoặc thông báo cho người dùng
                    Console.WriteLine($"SendMessage error: {ex.Message}");
                }
            }
        }
    }
}
