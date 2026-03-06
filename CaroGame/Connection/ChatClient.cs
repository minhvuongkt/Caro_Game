using Client.Connection;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Client.Constants
{
    public class ChatClient
    {
        public readonly ConnectToServer _connectToServer;
        public event Action<Chat> OnMessageReceived;

        public ChatClient()
        {
            _connectToServer = new ConnectToServer();
            MessageDispatcher.Instance.Register("Chat",     OnChatMessage);
            MessageDispatcher.Instance.Register("ChatList", OnChatListMessage);
        }

        private void OnChatMessage(Models.Message msg)
        {
            var chat = Unwrap<Chat>(msg.Data);
            if (chat != null) OnMessageReceived?.Invoke(chat);
        }

        private void OnChatListMessage(Models.Message msg)
        {
            var chatList = Unwrap<List<Chat>>(msg.Data);
            if (chatList == null) return;
            foreach (var c in chatList) DataCache.groupChatCache.Add(c);
        }

        public void SendMessage(Chat chat)
        {
            if (chat != null)
                _connectToServer.Send(new Models.Message { MessageType = "Chat", Data = chat });
        }

        private static T Unwrap<T>(object data)
        {
            if (data is JsonElement j) return JsonSerializer.Deserialize<T>(j.GetRawText());
            return default;
        }
    }
}
