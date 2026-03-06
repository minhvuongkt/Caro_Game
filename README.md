# Caro Game — by Miu2k3 (Minh Vương)

> 🇻🇳 [Tiếng Việt](#-tiếng-việt) · 🇬🇧 [English](#-english)

---

## 🇻🇳 Tiếng Việt

### Giới thiệu

**Caro Game** là ứng dụng game nhiều người chơi viết bằng C# .NET Framework 4.7.2, gồm:

- **Client** — WinForms chạy trên Windows
- **Server** — Console app lắng nghe kết nối TCP Socket (nâng cấp lên WebSocket qua `GameProxyClient` khi triển khai qua Cloudflare / nginx)
- **Cơ sở dữ liệu** — MySQL lưu người chơi, lịch sử ván đấu và tin nhắn

---

### 🎮 Các chế độ chơi có thể triển khai ngay

#### Offline (Không cần server)

| Chế độ | Mô tả |
|--------|-------|
| **Đấu với Bot Ngốc** | Bảng 3×3 — AI đơn giản, đi ngẫu nhiên |
| **Đấu với Bot Thường** | Bảng 3×3 — AI chặn nước thắng, suy nghĩ 3 giây |
| **PvP Offline** | Bảng 3×3 — 2 người chơi trên cùng một máy |
| Giới hạn thời gian mỗi lượt | 10 giây / 20 giây / 30 giây (bot tự đi khi hết giờ) |

> **Luật Offline:** Bảng 3×3, ai tạo được 3 quân liên tiếp thắng.  
> **Luật đặc biệt:** Sau mỗi 5 lượt, nước đi đầu tiên của bạn bị xóa — hãy tính toán cẩn thận!

#### Online / Nhiều người chơi

| Chức năng | Mô tả |
|-----------|-------|
| **Đăng ký tự động** | Lần đầu khởi động: nhập tên, server tự cấp UID (địa chỉ IP) |
| **Phòng chờ (Room Lobby)** | Xem danh sách phòng, tạo / tham gia / xem |
| **Caro 15×15** | 5 quân liên tiếp thắng — chế độ phổ biến nhất |
| **Cờ Go 19×19** | 5 quân liên tiếp thắng trên bảng 19×19 |
| **Caro 2v2 (15×15)** | Đội 2 vs đội 2, đi xen kẽ theo thứ tự |
| **Ghép trận ngẫu nhiên** | Xếp hàng chờ đối thủ, chấp nhận hoặc từ chối khi tìm được — xem chi tiết bên dưới |
| **Mời bạn vào phòng** | Mời theo UID — bạn có thể chấp nhận hoặc từ chối |
| **Chế độ Xem** | Vào xem bất kỳ ván đấu nào đang diễn ra |
| **Chat trong phòng** | Nhắn tin với đối thủ trong ván đấu |
| **Đồng bộ nước đi thời gian thực** | Server xác thực và phát lại cho tất cả người chơi |
| **Phát hiện thắng / hoà** | 5 quân liên tiếp theo 4 hướng |
| **Mất kết nối** | Đối thủ tự động thắng |
| **Tính điểm** | Thắng +30 / Thua −20 / Hoà +5 |
| **Bảng xếp hạng** | Xếp theo điểm (view `vw_leaderboard`) |
| **Lịch sử ván đấu** | Lưu toàn bộ ván vào cơ sở dữ liệu |

#### Hệ thống Chat

| Chức năng | Mô tả |
|-----------|-------|
| **Chat riêng (DM)** | Nhắn tin với bất kỳ người chơi nào theo UID |
| **Chat nhóm** | Gửi tin đến tất cả người đang kết nối |
| **Lịch sử chat** | Lưu vào DB, tải lại khi kết nối lại |

---

### 🔀 Cơ chế Ghép Trận Ngẫu Nhiên (Random Matchmaking)

#### Luồng hoạt động

```
Người chơi A                   Server                   Người chơi B
     │                            │                            │
     │──── MatchQueue (Caro) ────►│                            │
     │                            │◄─── MatchQueue (Caro) ────│
     │                            │                            │
     │◄─── MatchFound ────────────┤──── MatchFound ───────────►│
     │     (MatchID, tên B)       │     (MatchID, tên A)       │
     │                            │                            │
     │──── MatchAccept ──────────►│◄─── MatchAccept ──────────│
     │                            │  (cả 2 đồng ý → tạo phòng)│
     │◄─── MatchConfirmed ────────┤──── MatchConfirmed ───────►│
     │     (Room sẵn sàng)        │                            │
```

- Nếu **một trong hai từ chối** (`MatchDecline`): cả hai nhận `MatchCancelled`.
- Nếu người chơi **ngắt kết nối** trong khi chờ: tự động huỷ hàng đợi.
- Sử dụng `GameClient.QueueForMatch("Caro")` / `AcceptMatch(matchId)` / `DeclineMatch(matchId)`.

---

### 🌐 Kết nối WebSocket (`GameProxyClient`)

`GameProxyClient` là lớp transport WebSocket chuẩn để kết nối đến server khi triển khai thực tế (qua Cloudflare / nginx với bảo vệ DDoS và TLS tự động).

#### Ví dụ sử dụng

```csharp
var proxy = new GameProxyClient("wss://game.minhvuong.io.vn");

proxy.OnConnected    += ()    => Console.WriteLine("Đã kết nối!");
proxy.OnDisconnected += ()    => Console.WriteLine("Mất kết nối");
proxy.OnDataReceived += data  => ParseGamePacket(data);
proxy.OnError        += ex    => Console.WriteLine($"Lỗi: {ex.Message}");

await proxy.ConnectAsync();

// Gửi dữ liệu
await proxy.SendAsync(JsonSerializer.SerializeToUtf8Bytes(message));
```

#### So sánh TCP → WebSocket

| | TCP cũ (`ConnectToServer`) | WebSocket mới (`GameProxyClient`) |
|--|--|--|
| Giao thức | Raw TCP socket | WebSocket (ws:// hoặc wss://) |
| Triển khai LAN | ✅ | ✅ (ws://127.0.0.1:2003) |
| Triển khai Internet | ❌ (cần port forward) | ✅ (qua Cloudflare proxy) |
| TLS / DDoS bảo vệ | ❌ | ✅ |

---

### 🗄️ Cơ sở dữ liệu (MySQL)

| Bảng / View | Mô tả |
|-------------|-------|
| `Players` | UID, tên, điểm, thắng/thua/hoà |
| `Chats` | Tin nhắn riêng và nhóm với timestamp |
| `GameHistory` | Toàn bộ ván đấu (người chơi, số nước, ảnh bảng, thời gian) |
| `vw_leaderboard` | Top người chơi theo điểm |
| `vw_recent_games` | 100 ván gần nhất đã hoàn thành |

---

### ⚙️ Công nghệ

| | |
|--|--|
| Ngôn ngữ | C# .NET Framework 4.7.2 |
| Giao diện Client | WinForms |
| Server | Console App |
| Giao thức | TCP Socket (nội bộ) / WebSocket (triển khai) |
| Cơ sở dữ liệu | MySQL |
| Thư viện | Dapper, MySql.Data, System.Text.Json |
| Tác giả | **Miu2k3 — Minh Vương** |

---

---

## 🇬🇧 English

### Overview

**Caro Game** is a multiplayer board-game application built on C# .NET Framework 4.7.2, consisting of:

- **Client** — WinForms desktop app (Windows)
- **Server** — Console application listening on a TCP Socket (upgradeable to WebSocket via `GameProxyClient` when deployed behind Cloudflare / nginx)
- **Database** — MySQL storing players, match history, and chat messages

---

### 🎮 Playable Features (Ready to Deploy)

#### Offline (No server required)

| Mode | Description |
|------|-------------|
| **vs Stupid Bot** | 3×3 board — simple random AI |
| **vs Normal Bot** | 3×3 board — blocks winning lines, 3-second think time |
| **Local PvP** | 3×3 board — 2 players on the same machine |
| Turn time limit | 10 s / 20 s / 30 s (bot auto-plays when time runs out) |

> **Offline rules:** 3×3 board, first to get 3 in a row wins.  
> **Special rule:** Every 5 turns your earliest move is removed — plan carefully!

#### Online / Multiplayer

| Feature | Description |
|---------|-------------|
| **Auto-registration** | Enter a display name on first launch; server assigns UID (client IP) |
| **Room Lobby** | Browse, create, join, or spectate rooms |
| **Caro 15×15** | 5 consecutive pieces wins — the main online mode |
| **Go 19×19** | 5 consecutive pieces wins on a 19×19 board |
| **2v2 Caro 15×15** | Two teams of two, alternating turns |
| **Random Matchmaking** | Queue for an opponent; accept or decline when a match is found — see details below |
| **Room Invitation** | Invite a friend by UID — they can accept or decline |
| **Spectator Mode** | Watch any ongoing game live |
| **In-room Chat** | Chat with your opponent during a game |
| **Real-time Move Sync** | Server-authoritative; move broadcast to all participants |
| **Win / Draw Detection** | 5 consecutive pieces in any direction |
| **Disconnect Handling** | Opponent is awarded the win automatically |
| **Score Tracking** | Win +30 pts / Loss −20 pts / Draw +5 pts |
| **Leaderboard** | Ranked by score (database view `vw_leaderboard`) |
| **Match History** | Every completed game saved to the database |

#### Chat System

| Feature | Description |
|---------|-------------|
| **Private (DM) chat** | Message any player by UID |
| **Group chat** | Broadcast to all connected players |
| **Chat history** | Persisted in DB, reloaded on reconnect |

---

### 🔀 Random Matchmaking

#### Flow diagram

```
Player A                       Server                       Player B
   │                              │                              │
   │──── MatchQueue("Caro") ─────►│                              │
   │                              │◄──── MatchQueue("Caro") ────│
   │                              │                              │
   │◄─── MatchFound ──────────────┤──── MatchFound ─────────────►│
   │     (MatchID, B's name)      │     (MatchID, A's name)      │
   │                              │                              │
   │──── MatchAccept ────────────►│◄──── MatchAccept ───────────│
   │                              │  (both accepted → create room)│
   │◄─── MatchConfirmed ──────────┤──── MatchConfirmed ─────────►│
   │     (Room ready)             │                              │
```

- If **either player declines** (`MatchDecline`): both receive `MatchCancelled`.
- If a player **disconnects** while queued: the queue entry is cleaned up automatically.
- Client API: `GameClient.QueueForMatch("Caro")` / `AcceptMatch(matchId)` / `DeclineMatch(matchId)`.

##### Message types

| Direction | MessageType | Payload |
|-----------|-------------|---------|
| Client → Server | `RoomAction { Action="MatchQueue", GameType="Caro" }` | Queue for a game type |
| Client → Server | `RoomAction { Action="MatchCancel" }` | Leave the queue |
| Client → Server | `RoomAction { Action="MatchAccept", RoomID=matchId }` | Accept a found match |
| Client → Server | `RoomAction { Action="MatchDecline", RoomID=matchId }` | Decline a found match |
| Server → Client | `MatchQueued` | Confirmation you are in the queue |
| Server → Client | `MatchFound` | `MatchFoundData` — opponent info + MatchID |
| Server → Client | `MatchConfirmed` | `Room` object — game is starting |
| Server → Client | `MatchCancelled` | Reason string |

---

### 🌐 WebSocket Transport (`GameProxyClient`)

`GameProxyClient` is the standard WebSocket connection layer for production deployment behind a Cloudflare or nginx reverse-proxy (DDoS protection + automatic TLS).

#### Usage example

```csharp
var proxy = new GameProxyClient("wss://game.minhvuong.io.vn");

proxy.OnConnected    += ()    => Console.WriteLine("Connected!");
proxy.OnDisconnected += ()    => Console.WriteLine("Disconnected");
proxy.OnDataReceived += data  => ParseGamePacket(data);
proxy.OnError        += ex    => Console.WriteLine($"Error: {ex.Message}");

await proxy.ConnectAsync();

// Send a packet
await proxy.SendAsync(JsonSerializer.SerializeToUtf8Bytes(message));

// Clean up
await proxy.DisconnectAsync();
proxy.Dispose();
```

#### TCP → WebSocket migration guide

| | Old TCP (`ConnectToServer`) | New WebSocket (`GameProxyClient`) |
|--|--|--|
| Protocol | Raw TCP socket | WebSocket (ws:// / wss://) |
| LAN / local | ✅ | ✅ (`ws://127.0.0.1:2003`) |
| Internet deployment | ❌ (requires port forwarding) | ✅ (via Cloudflare proxy) |
| TLS / DDoS protection | ❌ | ✅ |
| API | `socket.Send(bytes)` (sync) | `await proxy.SendAsync(bytes)` (async) |
| Receive | `socket.Receive(buffer)` (blocking) | `OnDataReceived` event (non-blocking) |

---

### 🗄️ Database (MySQL)

| Table / View | Description |
|-------------|-------------|
| `Players` | UID, display name, score, wins / losses / draws |
| `Chats` | Private and group messages with timestamps |
| `GameHistory` | Full match record (players, move count, board snapshot, times) |
| `vw_leaderboard` | Top players ranked by score |
| `vw_recent_games` | Last 100 completed games |

---

### ⚙️ Tech Stack

| | |
|--|--|
| Language | C# .NET Framework 4.7.2 |
| Client UI | WinForms |
| Server | Console Application |
| Transport | TCP Socket (local) / WebSocket (production) |
| Database | MySQL |
| Libraries | Dapper, MySql.Data, System.Text.Json |
| Author | **Miu2k3 — Minh Vương** |

