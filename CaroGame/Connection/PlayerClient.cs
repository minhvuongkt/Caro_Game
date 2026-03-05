using Client.Constants;
using Client.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace Client.Connection
{
    public class PlayerClient
    {
        readonly ConnectToServer _connectToServer;

        public PlayerClient()
        {
            _connectToServer = new ConnectToServer();
            // Register with the central dispatcher instead of spinning up our own receive thread
            MessageDispatcher.Instance.Register("Player", OnPlayerMessage);
        }

        private void OnPlayerMessage(Models.Message msg)
        {
            var player = Unwrap<PlayerInfo>(msg.Data);
            if (player != null) DataCache.Player = player;
        }

        public void CreateNewPlayer(string namePlayer)
        {
            var player = new PlayerInfo { UID = "", Fullname = namePlayer, Friends = new List<Friend>(), Score = 1000 };
            _connectToServer.Send(new Models.Message { MessageType = "Player", Data = player });
        }

        public void GetPlayer()
        {
            _connectToServer.Send(new Models.Message { MessageType = "Player", Data = new PlayerInfo { UID = "GetPlayer" } });
        }

        public void UpdatePlayer(PlayerInfo player)
        {
            _connectToServer.Send(new Models.Message { MessageType = "Player", Data = player });
        }

        private static T Unwrap<T>(object data)
        {
            if (data is JsonElement j) return JsonSerializer.Deserialize<T>(j.GetRawText());
            return default;
        }
    }
}
