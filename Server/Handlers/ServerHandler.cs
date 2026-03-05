using Server.Constants;
using Server.Models;
using Server.SQLServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace Server.Handlers
{
    public class ServerHandler
    {
        // Score deltas awarded after a finished game
        private const int SCORE_WIN  =  30;
        private const int SCORE_LOSS = -20;
        private const int SCORE_DRAW =   5;

        private IPEndPoint IP { get; set; }
        private Socket server { get; set; }
        private List<Socket> clientList { get; set; }
        private Dictionary<string, Socket> clientDict { get; set; }
        private Dictionary<string, Socket> uidToSocket { get; set; }

        private readonly ChatDAL messageDAL;
        private readonly PlayerDAL playerDAL;
        private readonly GameHistoryDAL gameHistoryDAL;

        private List<Chat> groupChatMessages { get; set; } = new List<Chat>();
        private List<Socket> groupChatMembers { get; set; } = new List<Socket>();

        // Room state (in-memory, not persisted)
        private readonly Dictionary<string, Room> _rooms = new Dictionary<string, Room>();
        private readonly Dictionary<string, string> _uidToRoomId = new Dictionary<string, string>();
        // Maps roomId -> database GameHistory.ID for the active game
        private readonly Dictionary<string, int> _roomToGameHistoryId = new Dictionary<string, int>();
        // Maps roomId -> StartTime for the active game
        private readonly Dictionary<string, DateTime> _roomStartTime = new Dictionary<string, DateTime>();
        private readonly object _roomLock = new object();
        private int _roomCounter = 0;

        public ServerHandler()
        {
            playerDAL     = new PlayerDAL();
            messageDAL    = new ChatDAL();
            gameHistoryDAL = new GameHistoryDAL();
            KhoiTao();
        }

        private void KhoiTao()
        {
            clientList = new List<Socket>();
            clientDict = new Dictionary<string, Socket>();
            uidToSocket = new Dictionary<string, Socket>();
            IP = new IPEndPoint(IPAddress.Parse(Driver.gI().ServerIP), Driver.gI().ServerPort);
            StartServer();
        }

        private void StartServer()
        {
            if (server == null || !server.IsBound)
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Parse(Driver.gI().ServerIP), Driver.gI().ServerPort));
                Console.WriteLine($"Server started listening on port: {Driver.gI().ServerPort}");
            }
            Thread listenThread = new Thread(() =>
            {
                try
                {
                    server.Listen(100);
                    while (true)
                    {
                        Socket client = server.Accept();
                        clientList.Add(client);
                        clientDict[client.RemoteEndPoint.ToString()] = client;
                        Console.WriteLine($"{client.RemoteEndPoint} is connected!");
                        Thread receiveThread = new Thread(() => Receive(client));
                        receiveThread.IsBackground = true;
                        receiveThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server exception: {ex.Message}");
                    Close();
                }
            });
            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public void Close()
        {
            try { foreach (var c in clientList) c.Close(); server.Close(); Console.WriteLine("Server closed."); }
            catch (Exception ex) { Console.WriteLine($"Close exception: {ex.Message}"); }
            Environment.Exit(0);
        }

        public void Send(Socket client, object obj)
        {
            if (client == null || obj == null) return;
            try { client.Send(Serialize(obj)); }
            catch (Exception ex) { Console.WriteLine($"Send exception: {ex.Message}"); }
        }

        private void Receive(Socket client)
        {
            byte[] data = new byte[1024 * 5000];
            string endpointStr = client.RemoteEndPoint.ToString();
            string clientIP = endpointStr.Split(':')[0];
            try
            {
                while (true)
                {
                    int receivedBytes = client.Receive(data);
                    if (receivedBytes <= 0) continue;
                    byte[] validData = new byte[receivedBytes];
                    Array.Copy(data, validData, receivedBytes);
                    var message = Deserialize<Message>(validData);
                    if (message == null) continue;
                    switch (message.MessageType)
                    {
                        case "Player":
                        {
                            var player = Unwrap<Player>(message.Data);
                            if (player == null) break;
                            if (player.ID == 0 && player.UID != "GetPlayer")
                            {
                                player.UID = clientIP;
                                uidToSocket[clientIP] = client;
                                var existing = playerDAL.GetPlayerByUID(clientIP);
                                if (existing == null)
                                { if (playerDAL.AddPlayer(player)) Send(client, new Message { MessageType = "Player", Data = player }); }
                                else Send(client, new Message { MessageType = "Player", Data = existing });
                            }
                            else if (player.UID == "GetPlayer")
                            {
                                uidToSocket[clientIP] = client;
                                var p = playerDAL.GetPlayerByUID(clientIP);
                                if (p != null) Send(client, new Message { MessageType = "Player", Data = p });
                            }
                            else if (player.ID > 0) { playerDAL.UpdatePlayer(player); }
                            break;
                        }
                        case "Chat":
                        {
                            var chat = Unwrap<Chat>(message.Data);
                            if (chat == null) break;
                            if (chat.IsGroupChat)
                            {
                                if (!groupChatMembers.Contains(client)) groupChatMembers.Add(client);
                                groupChatMessages.Add(chat);
                                messageDAL.AddChat(chat);
                                BroadcastToGroupChat(client, chat);
                            }
                            else HandlePrivateChat(chat);
                            break;
                        }
                        case "MessageRequest":
                        {
                            var req = Unwrap<MessageRequest>(message.Data);
                            if (req != null) HandleMessageRequest(client, clientIP, req);
                            break;
                        }
                        case "RoomAction":
                        { var action = Unwrap<RoomAction>(message.Data); if (action != null) HandleRoomAction(client, clientIP, action); break; }
                        case "GameMove":
                        { var move = Unwrap<GameMove>(message.Data); if (move != null) HandleGameMove(clientIP, move); break; }
                        case "RoomChat":
                        { var chat = Unwrap<Chat>(message.Data); if (chat != null) HandleRoomChat(clientIP, chat); break; }
                        default: Console.WriteLine($"Unknown message type: {message.MessageType}"); break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receive exception ({clientIP}): {ex.Message}");
                clientList.Remove(client);
                clientDict.Remove(endpointStr);
                uidToSocket.Remove(clientIP);
                HandlePlayerDisconnect(clientIP);
                client.Close();
            }
        }

        // ── Group / private chat ──────────────────────────────────────────────

        private void BroadcastToGroupChat(Socket sender, Chat message)
        {
            foreach (var member in groupChatMembers)
            {
                try { if (member != sender) Send(member, new Message { MessageType = "Chat", Data = message }); }
                catch (Exception ex) { Console.WriteLine($"Broadcast exception: {ex.Message}"); }
            }
        }

        private void HandlePrivateChat(Chat message)
        {
            messageDAL.AddChat(message);
            if (clientDict.TryGetValue(message.ReceiverUID, out Socket recv))
                Send(recv, new Message { MessageType = "Chat", Data = message });
            else Console.WriteLine($"User {message.ReceiverUID} not connected (message saved to DB)");
        }

        // Fix: pass clientUID so private-chat history uses the correct UID
        private void HandleMessageRequest(Socket client, string clientUID, MessageRequest request)
        {
            if (request.FriendUID == "Group Chat")
            {
                var msgs = messageDAL.GetChatsByGroupSince(request.LastMessageTime).ToList();
                Send(client, new Message { MessageType = "ChatList", Data = msgs });
            }
            else
            {
                // SQL-level filter to avoid loading entire conversation into memory
                var msgs = messageDAL
                    .GetChatsBetweenUsersSince(clientUID, request.FriendUID, request.LastMessageTime)
                    .ToList();
                Send(client, new Message { MessageType = "ChatList", Data = msgs });
            }
        }

        // ── Room management ───────────────────────────────────────────────────

        private void HandleRoomAction(Socket client, string uid, RoomAction action)
        {
            switch (action.Action)
            {
                case "List":         SendRoomList(client); break;
                case "Create":       CreateRoom(client, uid, action); break;
                case "Join":         JoinRoomAsPlayer(client, uid, action.RoomID); break;
                case "Spectate":     JoinRoomAsSpectator(client, uid, action.RoomID); break;
                case "Leave":        LeaveRoom(uid); break;
                case "Invite":       InvitePlayer(uid, action); break;
                case "AcceptInvite": JoinRoomAsPlayer(client, uid, action.RoomID); break;
                case "DeclineInvite":
                    if (_rooms.TryGetValue(action.RoomID, out var rm) && uidToSocket.TryGetValue(rm.HostUID, out var hs))
                        Send(hs, new Message { MessageType = "InviteDeclined", Data = uid });
                    break;
            }
        }

        private void CreateRoom(Socket client, string uid, RoomAction action)
        {
            lock (_roomLock)
            {
                var gameType = ParseGameType(action.GameType);
                int boardSize = gameType == GameType.Go ? 19 : 15;
                string roomId = $"room_{++_roomCounter}";
                var room = new Room
                {
                    RoomID = roomId,
                    Name = string.IsNullOrEmpty(action.RoomName) ? $"{uid}'s Room" : action.RoomName,
                    HostUID = uid, Player1UID = uid,
                    GameType = gameType, Status = RoomStatus.Waiting,
                    BoardSize = boardSize, Board = new string[boardSize * boardSize],
                    CurrentTurnUID = uid, MoveCount = 0,
                    SpectatorUIDs = new List<string>()
                };
                for (int i = 0; i < room.Board.Length; i++) room.Board[i] = "";
                _rooms[roomId] = room;
                _uidToRoomId[uid] = roomId;
                Console.WriteLine($"Room created: {roomId} by {uid} ({gameType})");
                Send(client, new Message { MessageType = "Room", Data = room });
                BroadcastRoomListToAll();
            }
        }

        private void JoinRoomAsPlayer(Socket client, string uid, string roomId)
        {
            lock (_roomLock)
            {
                if (!_rooms.TryGetValue(roomId, out var room) || room.Status != RoomStatus.Waiting) return;
                bool joined = false;
                if (string.IsNullOrEmpty(room.Player2UID) && room.Player1UID != uid)
                { room.Player2UID = uid; joined = true; }
                else if (room.GameType == GameType.TwoVsTwo)
                {
                    if (string.IsNullOrEmpty(room.Team1AllyUID) && uid != room.Player1UID && uid != room.Player2UID)
                    { room.Team1AllyUID = uid; joined = true; }
                    else if (string.IsNullOrEmpty(room.Team2AllyUID) && uid != room.Player1UID && uid != room.Player2UID && uid != room.Team1AllyUID)
                    { room.Team2AllyUID = uid; joined = true; }
                }
                if (!joined) return;
                _uidToRoomId[uid] = roomId;
                bool full = room.GameType == GameType.TwoVsTwo
                    ? !string.IsNullOrEmpty(room.Player1UID) && !string.IsNullOrEmpty(room.Player2UID)
                      && !string.IsNullOrEmpty(room.Team1AllyUID) && !string.IsNullOrEmpty(room.Team2AllyUID)
                    : !string.IsNullOrEmpty(room.Player1UID) && !string.IsNullOrEmpty(room.Player2UID);
                if (full)
                {
                    room.Status = RoomStatus.Playing;
                    room.CurrentTurnUID = room.Player1UID;
                    // Record game start in database
                    try
                    {
                        var startTime = DateTime.UtcNow;
                        _roomStartTime[roomId] = startTime;
                        int histId = gameHistoryDAL.AddGameHistory(new GameHistory
                        {
                            RoomID       = roomId,
                            GameType     = room.GameType,
                            Player1UID   = room.Player1UID,
                            Player2UID   = room.Player2UID ?? "",
                            Team1AllyUID = room.Team1AllyUID,
                            Team2AllyUID = room.Team2AllyUID,
                            StartTime    = startTime
                        });
                        _roomToGameHistoryId[roomId] = histId;
                    }
                    catch (Exception ex) { Console.WriteLine($"DB error saving game start: {ex.Message}"); }
                }
                BroadcastToRoom(room, new Message { MessageType = "GameState", Data = room });
                BroadcastRoomListToAll();
            }
        }

        private void JoinRoomAsSpectator(Socket client, string uid, string roomId)
        {
            lock (_roomLock)
            {
                if (!_rooms.TryGetValue(roomId, out var room)) return;
                if (!room.SpectatorUIDs.Contains(uid)) room.SpectatorUIDs.Add(uid);
                Send(client, new Message { MessageType = "GameState", Data = room });
            }
        }

        private void LeaveRoom(string uid)
        {
            lock (_roomLock)
            {
                if (!_uidToRoomId.TryGetValue(uid, out var roomId)) return;
                if (!_rooms.TryGetValue(roomId, out var room)) return;
                _uidToRoomId.Remove(uid);
                room.SpectatorUIDs.Remove(uid);
                bool wasPlayer = room.Player1UID == uid || room.Player2UID == uid
                                 || room.Team1AllyUID == uid || room.Team2AllyUID == uid;
                if (wasPlayer && room.Status == RoomStatus.Playing)
                {
                    room.Status = RoomStatus.Finished;
                    bool leaverIsTeam1 = room.Player1UID == uid || room.Team1AllyUID == uid;
                    room.WinnerUID = leaverIsTeam1
                        ? (room.Player2UID ?? room.Team2AllyUID)
                        : (room.Player1UID ?? room.Team1AllyUID);
                    BroadcastToRoom(room, new Message { MessageType = "GameState", Data = room });
                    FinalizeGame(room);
                }
                if (room.Player1UID == uid) room.Player1UID = null;
                else if (room.Player2UID == uid) room.Player2UID = null;
                else if (room.Team1AllyUID == uid) room.Team1AllyUID = null;
                else if (room.Team2AllyUID == uid) room.Team2AllyUID = null;
                if (string.IsNullOrEmpty(room.Player1UID) && string.IsNullOrEmpty(room.Player2UID)
                    && room.SpectatorUIDs.Count == 0)
                    CleanupRoom(roomId);
                BroadcastRoomListToAll();
            }
        }

        private void InvitePlayer(string fromUID, RoomAction action)
        {
            if (!_rooms.TryGetValue(action.RoomID, out var room)) return;
            if (!uidToSocket.TryGetValue(action.TargetUID, out var target)) return;
            Send(target, new Message
            {
                MessageType = "RoomInvite",
                Data = new RoomInviteData
                {
                    FromUID  = fromUID,
                    RoomID   = room.RoomID,
                    RoomName = room.Name,
                    GameType = room.GameType.ToString()
                }
            });
        }

        private void HandlePlayerDisconnect(string uid)
        {
            lock (_roomLock) { if (_uidToRoomId.ContainsKey(uid)) LeaveRoom(uid); }
        }

        // ── Game logic ────────────────────────────────────────────────────────

        private void HandleGameMove(string uid, GameMove move)
        {
            lock (_roomLock)
            {
                if (!_rooms.TryGetValue(move.RoomID, out var room)
                    || room.Status != RoomStatus.Playing
                    || room.CurrentTurnUID != uid) return;

                int idx = move.Row * room.BoardSize + move.Col;
                if (idx < 0 || idx >= room.Board.Length || !string.IsNullOrEmpty(room.Board[idx])) return;

                string piece = GetPieceForPlayer(room, uid);
                room.Board[idx] = piece;
                room.MoveCount++;

                bool won  = CheckWin(room.Board, room.BoardSize, move.Row, move.Col, piece);
                bool draw = room.MoveCount >= room.Board.Length;

                if (won)  { room.Status = RoomStatus.Finished; room.WinnerUID = uid; }
                else if (draw) { room.Status = RoomStatus.Finished; room.WinnerUID = "draw"; }
                else room.CurrentTurnUID = NextTurn(room, uid);

                BroadcastToRoom(room, new Message { MessageType = "GameState", Data = room });

                if (room.Status == RoomStatus.Finished)
                    FinalizeGame(room);
            }
        }

        private void HandleRoomChat(string uid, Chat chat)
        {
            lock (_roomLock)
            {
                if (!_uidToRoomId.TryGetValue(uid, out var roomId)) return;
                if (!_rooms.TryGetValue(roomId, out var room)) return;
                bool isPlayer = room.Player1UID == uid || room.Player2UID == uid
                                || room.Team1AllyUID == uid || room.Team2AllyUID == uid;
                if (!isPlayer) return;
                BroadcastToRoom(room, new Message { MessageType = "RoomChat", Data = chat });
            }
        }

        // ── Finalise a finished game ──────────────────────────────────────────

        private void FinalizeGame(Room room)
        {
            try
            {
                DateTime endTime      = DateTime.UtcNow;
                string   boardJson    = JsonSerializer.Serialize(room.Board);

                // Persist to GameHistory
                if (_roomToGameHistoryId.TryGetValue(room.RoomID, out int histId))
                {
                    gameHistoryDAL.FinishGame(histId, room.WinnerUID, room.MoveCount, endTime, boardJson);
                    _roomToGameHistoryId.Remove(room.RoomID);
                }

                _roomStartTime.Remove(room.RoomID);

                // Update player scores
                if (room.GameType == GameType.TwoVsTwo)
                {
                    var team1 = new[] { room.Player1UID, room.Team1AllyUID }.Where(u => !string.IsNullOrEmpty(u));
                    var team2 = new[] { room.Player2UID, room.Team2AllyUID }.Where(u => !string.IsNullOrEmpty(u));

                    bool team1Won = team1.Contains(room.WinnerUID);
                    bool isDraw   = room.WinnerUID == "draw";

                    foreach (var uid in team1)
                        ApplyScoreDelta(uid, isDraw ? SCORE_DRAW : team1Won ? SCORE_WIN : SCORE_LOSS,
                                        isDraw ? 0 : team1Won ? 1 : 0,
                                        isDraw ? 0 : team1Won ? 0 : 1,
                                        isDraw ? 1 : 0);
                    foreach (var uid in team2)
                        ApplyScoreDelta(uid, isDraw ? SCORE_DRAW : team1Won ? SCORE_LOSS : SCORE_WIN,
                                        isDraw ? 0 : team1Won ? 0 : 1,
                                        isDraw ? 0 : team1Won ? 1 : 0,
                                        isDraw ? 1 : 0);
                }
                else
                {
                    bool isDraw = room.WinnerUID == "draw";
                    string p1   = room.Player1UID;
                    string p2   = room.Player2UID;
                    if (string.IsNullOrEmpty(p1) || string.IsNullOrEmpty(p2)) return;

                    bool p1Won = room.WinnerUID == p1;
                    ApplyScoreDelta(p1, isDraw ? SCORE_DRAW : p1Won ? SCORE_WIN  : SCORE_LOSS,
                                    isDraw ? 0 : p1Won ? 1 : 0,
                                    isDraw ? 0 : p1Won ? 0 : 1,
                                    isDraw ? 1 : 0);
                    ApplyScoreDelta(p2, isDraw ? SCORE_DRAW : p1Won ? SCORE_LOSS : SCORE_WIN,
                                    isDraw ? 0 : p1Won ? 0 : 1,
                                    isDraw ? 0 : p1Won ? 1 : 0,
                                    isDraw ? 1 : 0);
                }
            }
            catch (Exception ex) { Console.WriteLine($"FinalizeGame error: {ex.Message}"); }
        }

        private void ApplyScoreDelta(string uid, int score, int wins, int losses, int draws)
        {
            try
            {
                playerDAL.UpdateScore(uid, score, wins, losses, draws);
                // Send refreshed player data back to client so the UI updates
                var updated = playerDAL.GetPlayerByUID(uid);
                if (updated != null && uidToSocket.TryGetValue(uid, out var sock))
                    Send(sock, new Message { MessageType = "Player", Data = updated });
            }
            catch (Exception ex) { Console.WriteLine($"ApplyScoreDelta error ({uid}): {ex.Message}"); }
        }

        private void CleanupRoom(string roomId)
        {
            _rooms.Remove(roomId);
            _roomToGameHistoryId.Remove(roomId);
            _roomStartTime.Remove(roomId);
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private static string GetPieceForPlayer(Room room, string uid)
        {
            if (uid == room.Player1UID || uid == room.Team1AllyUID) return "X";
            if (uid == room.Player2UID || uid == room.Team2AllyUID) return "O";
            return "";
        }

        private static string NextTurn(Room room, string currentUID)
        {
            if (room.GameType == GameType.TwoVsTwo)
            {
                var order = new[] { room.Player1UID, room.Player2UID, room.Team1AllyUID, room.Team2AllyUID };
                int idx = Array.IndexOf(order, currentUID);
                for (int i = 1; i <= order.Length; i++)
                { string next = order[(idx + i) % order.Length]; if (!string.IsNullOrEmpty(next)) return next; }
                return room.Player1UID;
            }
            return currentUID == room.Player1UID ? room.Player2UID : room.Player1UID;
        }

        private static bool CheckWin(string[] board, int boardSize, int row, int col, string piece, int winLength = 5)
        {
            if (string.IsNullOrEmpty(piece)) return false;
            int[][] dirs = { new[] { 0, 1 }, new[] { 1, 0 }, new[] { 1, 1 }, new[] { 1, -1 } };
            foreach (var d in dirs)
            {
                int count = 1;
                for (int i = 1; i < winLength; i++) { int r = row + i * d[0], c = col + i * d[1]; if (r < 0 || r >= boardSize || c < 0 || c >= boardSize || board[r * boardSize + c] != piece) break; count++; }
                for (int i = 1; i < winLength; i++) { int r = row - i * d[0], c = col - i * d[1]; if (r < 0 || r >= boardSize || c < 0 || c >= boardSize || board[r * boardSize + c] != piece) break; count++; }
                if (count >= winLength) return true;
            }
            return false;
        }

        private void BroadcastToRoom(Room room, Message msg)
        {
            var uids = new List<string>();
            if (!string.IsNullOrEmpty(room.Player1UID)) uids.Add(room.Player1UID);
            if (!string.IsNullOrEmpty(room.Player2UID)) uids.Add(room.Player2UID);
            if (!string.IsNullOrEmpty(room.Team1AllyUID)) uids.Add(room.Team1AllyUID);
            if (!string.IsNullOrEmpty(room.Team2AllyUID)) uids.Add(room.Team2AllyUID);
            uids.AddRange(room.SpectatorUIDs);
            foreach (var uid in uids) if (uidToSocket.TryGetValue(uid, out var s)) Send(s, msg);
        }

        private void SendRoomList(Socket client)
        { lock (_roomLock) { Send(client, new Message { MessageType = "RoomList", Data = _rooms.Values.ToList() }); } }

        private void BroadcastRoomListToAll()
        {
            var msg = new Message { MessageType = "RoomList", Data = _rooms.Values.ToList() };
            foreach (var uid in uidToSocket.Keys.ToList()) if (uidToSocket.TryGetValue(uid, out var s)) Send(s, msg);
        }

        private static GameType ParseGameType(string s)
        {
            if (string.IsNullOrEmpty(s)) return GameType.Caro;
            switch (s.ToLowerInvariant())
            { case "go": return GameType.Go; case "twovstwo": case "2v2": return GameType.TwoVsTwo; default: return GameType.Caro; }
        }

        private byte[] Serialize<T>(T obj) => JsonSerializer.SerializeToUtf8Bytes(obj);
        private T Deserialize<T>(byte[] data) => JsonSerializer.Deserialize<T>(data);
        private T Unwrap<T>(object data)
        {
            if (data is JsonElement j) return JsonSerializer.Deserialize<T>(j.GetRawText());
            throw new InvalidCastException($"Cannot unwrap to {typeof(T).Name}");
        }

        public void RemoveClient(Socket client)
        {
            clientList.Remove(client);
            clientDict.Remove(client.RemoteEndPoint.ToString());
            client.Disconnect(false);
            groupChatMembers.Remove(client);
        }
    }
}
