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

        // ── Matchmaking state (in-memory) ─────────────────────────────────────
        // Per-game-type FIFO queue of UIDs waiting for a random match
        private readonly Dictionary<string, Queue<string>> _matchQueues
            = new Dictionary<string, Queue<string>>(StringComparer.OrdinalIgnoreCase);
        // O(1) membership test: all UIDs currently in any matchmaking queue
        private readonly HashSet<string> _matchQueuedUids = new HashSet<string>();
        // Pending matches awaiting both players to accept
        private readonly Dictionary<string, PendingMatch> _pendingMatches
            = new Dictionary<string, PendingMatch>();
        // Reverse index: UID → current matchID (if any)
        private readonly Dictionary<string, string> _uidToMatchId
            = new Dictionary<string, string>();
        private int _matchCounter = 0;
        // In-memory player-name cache — populated on login to avoid DB calls inside _roomLock
        private readonly Dictionary<string, string> _playerNameCache
            = new Dictionary<string, string>();

        private class PendingMatch
        {
            public string MatchID;
            public string Player1UID;
            public string Player2UID;
            public string GameType;
            public bool   Player1Accepted;
            public bool   Player2Accepted;
        }

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
                                {
                                    if (playerDAL.AddPlayer(player))
                                    {
                                        _playerNameCache[clientIP] = player.Fullname;
                                        Send(client, new Message { MessageType = "Player", Data = player });
                                    }
                                }
                                else
                                {
                                    _playerNameCache[clientIP] = existing.Fullname;
                                    Send(client, new Message { MessageType = "Player", Data = existing });
                                }
                            }
                            else if (player.UID == "GetPlayer")
                            {
                                uidToSocket[clientIP] = client;
                                var p = playerDAL.GetPlayerByUID(clientIP);
                                if (p != null)
                                {
                                    _playerNameCache[clientIP] = p.Fullname;
                                    Send(client, new Message { MessageType = "Player", Data = p });
                                }
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
                case "List":          SendRoomList(client); break;
                case "Create":        CreateRoom(client, uid, action); break;
                case "Join":          JoinRoomAsPlayer(client, uid, action.RoomID); break;
                case "Spectate":      JoinRoomAsSpectator(client, uid, action.RoomID); break;
                case "Leave":         LeaveRoom(uid); break;
                case "Invite":        InvitePlayer(uid, action); break;
                case "AcceptInvite":  JoinRoomAsPlayer(client, uid, action.RoomID); break;
                case "DeclineInvite":
                    if (_rooms.TryGetValue(action.RoomID, out var rm) && uidToSocket.TryGetValue(rm.HostUID, out var hs))
                        Send(hs, new Message { MessageType = "InviteDeclined", Data = uid });
                    break;
                // ── Matchmaking ───────────────────────────────────────────────
                case "MatchQueue":    EnqueueForMatchmaking(uid, action.GameType ?? "Caro"); break;
                case "MatchCancel":   CancelMatchmaking(uid); break;
                case "MatchAccept":   AcceptMatch(uid, action.RoomID); break;
                case "MatchDecline":  DeclineMatch(uid, action.RoomID); break;
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
            lock (_roomLock)
            {
                if (_uidToRoomId.ContainsKey(uid)) LeaveRoom(uid);
                CancelMatchmaking(uid);
            }
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

        // ── Matchmaking helpers ───────────────────────────────────────────────

        /// <summary>
        /// Adds <paramref name="uid"/> to the matchmaking queue for <paramref name="gameType"/>.
        /// If a second player is already waiting, a pending match is created and both are notified.
        /// </summary>
        private void EnqueueForMatchmaking(string uid, string gameType)
        {
            lock (_roomLock)
            {
                // A player already in a room cannot queue
                if (_uidToRoomId.ContainsKey(uid)) return;
                // Remove from any previous queue / pending match first (idempotent re-queue)
                RemoveFromQueues(uid);

                if (!_matchQueues.TryGetValue(gameType, out var queue))
                {
                    queue = new Queue<string>();
                    _matchQueues[gameType] = queue;
                }

                // O(1) duplicate check via _matchQueuedUids
                if (!_matchQueuedUids.Contains(uid))
                {
                    queue.Enqueue(uid);
                    _matchQueuedUids.Add(uid);
                }

                Console.WriteLine($"Matchmaking: {uid} queued for {gameType} (queue size: {queue.Count})");

                // Notify the queueing player that they are now in the queue
                if (uidToSocket.TryGetValue(uid, out var sock))
                    Send(sock, new Message { MessageType = "MatchQueued", Data = gameType });

                // Try to pair two players
                if (queue.Count >= 2)
                {
                    string p1 = queue.Dequeue();
                    string p2 = queue.Dequeue();
                    _matchQueuedUids.Remove(p1);
                    _matchQueuedUids.Remove(p2);

                    string matchId = $"match_{++_matchCounter}";

                    var pending = new PendingMatch
                    {
                        MatchID  = matchId,
                        Player1UID = p1,
                        Player2UID = p2,
                        GameType = gameType
                    };
                    _pendingMatches[matchId] = pending;
                    _uidToMatchId[p1] = matchId;
                    _uidToMatchId[p2] = matchId;

                    Console.WriteLine($"Matchmaking: match found {matchId} — {p1} vs {p2} ({gameType})");

                    // Use cached names — no DB I/O while holding the lock
                    string p1Name = GetCachedPlayerName(p1);
                    string p2Name = GetCachedPlayerName(p2);

                    NotifyMatchFound(p1, matchId, opponentUID: p2, opponentName: p2Name, gameType);
                    NotifyMatchFound(p2, matchId, opponentUID: p1, opponentName: p1Name, gameType);
                }
            }
        }

        /// <summary>
        /// Removes <paramref name="uid"/> from all matchmaking queues and cancels any pending match
        /// they are part of (notifying the opponent).
        /// </summary>
        private void CancelMatchmaking(string uid)
        {
            lock (_roomLock)
            {
                RemoveFromQueues(uid);

                if (_uidToMatchId.TryGetValue(uid, out var matchId))
                    DeclineMatch(uid, matchId, notifyRequester: false);
            }
        }

        /// <summary>
        /// Called when a player accepts a pending match.
        /// If both players have accepted, a room is created and both receive MatchConfirmed.
        /// </summary>
        private void AcceptMatch(string uid, string matchId)
        {
            lock (_roomLock)
            {
                if (!_pendingMatches.TryGetValue(matchId, out var m)) return;

                bool isP1 = m.Player1UID == uid;
                if (isP1) m.Player1Accepted = true;
                else if (m.Player2UID == uid) m.Player2Accepted = true;
                else return; // not a participant

                Console.WriteLine($"Matchmaking: {uid} accepted {matchId}");

                if (m.Player1Accepted && m.Player2Accepted)
                {
                    // Both accepted — create the room
                    _uidToMatchId.Remove(m.Player1UID);
                    _uidToMatchId.Remove(m.Player2UID);
                    _pendingMatches.Remove(matchId);

                    var fakeAction = new RoomAction
                    {
                        GameType = m.GameType,
                        RoomName = $"Match {matchId}"
                    };
                    // Create room with Player1 as host
                    if (!uidToSocket.TryGetValue(m.Player1UID, out var s1)) return;
                    CreateRoom(s1, m.Player1UID, fakeAction);

                    // The room was just added; find it
                    if (!_uidToRoomId.TryGetValue(m.Player1UID, out var roomId)) return;
                    if (!uidToSocket.TryGetValue(m.Player2UID, out var s2)) return;

                    // Join Player2
                    JoinRoomAsPlayer(s2, m.Player2UID, roomId);

                    // Notify both so the UI can navigate to the game
                    if (_rooms.TryGetValue(roomId, out var room))
                    {
                        BroadcastToRoom(room, new Message { MessageType = "MatchConfirmed", Data = room });
                        Console.WriteLine($"Matchmaking: {matchId} confirmed → room {roomId}");
                    }
                }
            }
        }

        /// <summary>
        /// Called when a player declines a pending match (or when CancelMatchmaking cleans up).
        /// Notifies both players; the match is discarded.
        /// Safe to call with or without <c>_roomLock</c> already held (C# Monitor is re-entrant).
        /// </summary>
        private void DeclineMatch(string uid, string matchId, bool notifyRequester = true)
        {
            lock (_roomLock)
            {
                if (!_pendingMatches.TryGetValue(matchId, out var m)) return;

                string opponent = m.Player1UID == uid ? m.Player2UID : m.Player1UID;

                _uidToMatchId.Remove(m.Player1UID);
                _uidToMatchId.Remove(m.Player2UID);
                _pendingMatches.Remove(matchId);

                Console.WriteLine($"Matchmaking: {uid} declined {matchId}");

                if (notifyRequester && uidToSocket.TryGetValue(uid, out var declinerSock))
                    Send(declinerSock, new Message { MessageType = "MatchCancelled", Data = "You declined the match." });

                if (uidToSocket.TryGetValue(opponent, out var oppSock))
                    Send(oppSock, new Message { MessageType = "MatchCancelled", Data = "Opponent declined the match." });
            }
        }

        private void NotifyMatchFound(string uid, string matchId,
                                      string opponentUID, string opponentName, string gameType)
        {
            if (!uidToSocket.TryGetValue(uid, out var sock)) return;
            Send(sock, new Message
            {
                MessageType = "MatchFound",
                Data = new MatchFoundData
                {
                    MatchID      = matchId,
                    OpponentUID  = opponentUID,
                    OpponentName = opponentName,
                    GameType     = gameType
                }
            });
        }

        /// <summary>
        /// Removes <paramref name="uid"/> from every game-type queue.
        /// O(n) rebuild is only triggered when the uid is actually queued (guarded by O(1) HashSet).
        /// </summary>
        private void RemoveFromQueues(string uid)
        {
            if (!_matchQueuedUids.Remove(uid)) return; // not queued — nothing to do

            foreach (var queue in _matchQueues.Values)
            {
                if (!queue.Contains(uid)) continue;
                var items = queue.ToArray();
                queue.Clear();
                foreach (var item in items)
                    if (item != uid) queue.Enqueue(item);
                break; // a UID can only appear in one game-type queue
            }
        }

        private string GetCachedPlayerName(string uid)
        {
            return _playerNameCache.TryGetValue(uid, out var name) ? name : uid;
        }

        private string TryGetPlayerName(string uid)
        {
            if (_playerNameCache.TryGetValue(uid, out var cached)) return cached;
            try { return playerDAL.GetPlayerByUID(uid)?.Fullname ?? uid; }
            catch { return uid; }
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
