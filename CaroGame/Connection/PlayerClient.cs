using Client.Constants;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Forms;

namespace Client.Connection
{
    public class PlayerClient
    {
        readonly ConnectToServer _connectToServer;
        public PlayerClient()
        {
            _connectToServer = new ConnectToServer();
        }

        public void Receive()
        {
            try
            {
                while (true)
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
                            if (receivedObj.MessageType == "Player")
                            {
                                DataCache.Player = DeserializeFromJsonElement<PlayerInfo>(receivedObj.Data);
                            }
                        }
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show($"Error At: {er.Message}\n{er.StackTrace}", "Cảnh Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        //Tạo player mặc định trên 1 máy - duy nhất 1 player. Tên, điểm, UID
        public void CreateNewPlayer(string namePlayer)
        {
            var player = new PlayerInfo()
            {
                UID = "",
                Fullname = namePlayer,
                Friends = new List<Friend>(),
                Score = 1000
            };
            //Todo send to server
            var msg = new Models.Message()
            {
                MessageType = "Player",
                Data = player
            };
            _connectToServer.Send(msg);
        }
        public void GetPlayer()
        {
            var player = new PlayerInfo()
            {
                UID = "GetPlayer"
            };
            //Todo send to server
            var msg = new Models.Message()
            {
                MessageType = "Player",
                Data = player
            };
            _connectToServer.Send(msg);
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
        public void UpdatePlayer(PlayerInfo player)
        {
            var msg = new Models.Message()
            {
                MessageType = "Player",
                Data = player
            };
            _connectToServer.Send(msg);
        }

    }
}
