using Client.Constants;
using Client.Models;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Client.Connection
{
    /// <summary>
    /// Handles all game/room related messages from the server.
    /// Uses the shared MessageDispatcher to avoid socket read races.
    /// </summary>
    public sealed class GameClient
    {
        private static readonly GameClient _instance = new GameClient();
        public static GameClient Instance => _instance;

        private readonly ConnectToServer _connectToServer;

        // Events raised on the dispatcher thread – callers must marshal to UI thread
        public event Action<List<Room>> OnRoomListReceived;
        public event Action<Room>       OnRoomJoined;
        public event Action<Room>       OnGameStateUpdated;
        public event Action<RoomInviteData> OnRoomInviteReceived;
        public event Action<Chat>       OnRoomChatReceived;
        public event Action<string>     OnInviteDeclined; // payload = decliner UID

        private GameClient()
        {
            _connectToServer = new ConnectToServer();
            MessageDispatcher.Instance.Register("RoomList",      OnRoomListMsg);
            MessageDispatcher.Instance.Register("Room",          OnRoomMsg);
            MessageDispatcher.Instance.Register("GameState",     OnGameStateMsg);
            MessageDispatcher.Instance.Register("RoomInvite",    OnRoomInviteMsg);
            MessageDispatcher.Instance.Register("RoomChat",      OnRoomChatMsg);
            MessageDispatcher.Instance.Register("InviteDeclined",OnInviteDeclinedMsg);
        }

        // ── Inbound handlers ─────────────────────────────────────────────────

        private void OnRoomListMsg(Models.Message msg)
        {
            var rooms = Unwrap<List<Room>>(msg.Data);
            if (rooms == null) return;
            DataCache.Rooms = rooms;
            OnRoomListReceived?.Invoke(rooms);
        }

        private void OnRoomMsg(Models.Message msg)
        {
            var room = Unwrap<Room>(msg.Data);
            if (room == null) return;
            DataCache.CurrentRoom = room;
            OnRoomJoined?.Invoke(room);
        }

        private void OnGameStateMsg(Models.Message msg)
        {
            var room = Unwrap<Room>(msg.Data);
            if (room == null) return;
            DataCache.CurrentRoom = room;
            OnGameStateUpdated?.Invoke(room);
        }

        private void OnRoomInviteMsg(Models.Message msg)
        {
            var invite = Unwrap<RoomInviteData>(msg.Data);
            if (invite != null) OnRoomInviteReceived?.Invoke(invite);
        }

        private void OnRoomChatMsg(Models.Message msg)
        {
            var chat = Unwrap<Chat>(msg.Data);
            if (chat != null) OnRoomChatReceived?.Invoke(chat);
        }

        private void OnInviteDeclinedMsg(Models.Message msg)
        {
            var uid = msg.Data?.ToString();
            OnInviteDeclined?.Invoke(uid);
        }

        // ── Outbound helpers ─────────────────────────────────────────────────

        public void RequestRoomList()
            => _connectToServer.Send(new Models.Message { MessageType = "RoomAction", Data = new RoomAction { Action = "List" } });

        public void CreateRoom(string name, string gameType)
            => _connectToServer.Send(new Models.Message { MessageType = "RoomAction", Data = new RoomAction { Action = "Create", RoomName = name, GameType = gameType } });

        public void JoinRoom(string roomId)
            => _connectToServer.Send(new Models.Message { MessageType = "RoomAction", Data = new RoomAction { Action = "Join", RoomID = roomId } });

        public void SpectateRoom(string roomId)
            => _connectToServer.Send(new Models.Message { MessageType = "RoomAction", Data = new RoomAction { Action = "Spectate", RoomID = roomId } });

        public void LeaveRoom()
        {
            var roomId = DataCache.CurrentRoom?.RoomID;
            if (string.IsNullOrEmpty(roomId)) return;
            _connectToServer.Send(new Models.Message { MessageType = "RoomAction", Data = new RoomAction { Action = "Leave", RoomID = roomId } });
            DataCache.CurrentRoom = null;
            DataCache.IsSpectating = false;
        }

        public void InvitePlayer(string targetUID)
        {
            var roomId = DataCache.CurrentRoom?.RoomID;
            if (string.IsNullOrEmpty(roomId)) return;
            _connectToServer.Send(new Models.Message { MessageType = "RoomAction", Data = new RoomAction { Action = "Invite", RoomID = roomId, TargetUID = targetUID } });
        }

        public void AcceptInvite(string roomId)
            => _connectToServer.Send(new Models.Message { MessageType = "RoomAction", Data = new RoomAction { Action = "AcceptInvite", RoomID = roomId } });

        public void DeclineInvite(string roomId)
            => _connectToServer.Send(new Models.Message { MessageType = "RoomAction", Data = new RoomAction { Action = "DeclineInvite", RoomID = roomId } });

        public void SendGameMove(string roomId, int row, int col)
            => _connectToServer.Send(new Models.Message { MessageType = "GameMove", Data = new GameMove { RoomID = roomId, PlayerUID = DataCache.Player?.UID, Row = row, Col = col } });

        public void SendRoomChat(string roomId, string message)
        {
            var chat = new Chat { SenderUID = DataCache.Player?.UID, ReceiverUID = roomId, Message = message, Time = DateTime.Now, IsGroupChat = false };
            _connectToServer.Send(new Models.Message { MessageType = "RoomChat", Data = chat });
        }

        // ── Util ─────────────────────────────────────────────────────────────

        private static T Unwrap<T>(object data)
        {
            if (data is JsonElement j) return JsonSerializer.Deserialize<T>(j.GetRawText());
            return default;
        }
    }
}
