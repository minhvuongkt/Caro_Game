# Caro Game - Developed by Miu2k3

## ✅ Stable & Complete Features

### 1. Offline Game Modes

| Feature | Status |
|---------|--------|
| **vs Stupid Bot** (3×3 Tic-Tac-Toe, easy AI) | ✅ Complete |
| **vs Normal Bot** (3×3 Tic-Tac-Toe, harder AI — blocks winning lines, 3s think time) | ✅ Complete |
| **Local PvP** (3×3 Tic-Tac-Toe, 2 players on the same machine) | ✅ Complete |
| Time limit per turn — 10s / 20s / 30s (bot plays when time runs out) | ✅ Complete |

> **Offline rules:** 3×3 board, first to get 3 in a row wins.  
> **Special rule:** After every 5 turns, your earliest move is removed — plan carefully!

---

### 2. Online / Multiplayer Mode

| Feature | Status |
|---------|--------|
| Auto-registration on first launch (enter your display name; UID assigned by server) | ✅ Complete |
| **Room Lobby** — browse, create, join, or spectate rooms | ✅ Complete |
| **Caro 15×15** online match (5 in a row to win) | ✅ Complete |
| **Go 19×19** online match (5 in a row to win) | ✅ Complete |
| **2v2 Caro 15×15** team match | ✅ Complete |
| Real-time move synchronisation (server-authoritative) | ✅ Complete |
| Win / draw detection (5 consecutive pieces in any direction) | ✅ Complete |
| **Spectator mode** — watch any ongoing game live | ✅ Complete |
| **Room invitation** — invite a friend by UID into your room | ✅ Complete |
| **In-room chat** — chat with your opponent during a game | ✅ Complete |
| Player disconnects — opponent is awarded the win automatically | ✅ Complete |
| **Score tracking** — Win +30 pts / Loss -20 pts / Draw +5 pts | ✅ Complete |
| **Leaderboard** — ranked by score (database view `vw_leaderboard`) | ✅ Complete |
| **Game history** — every completed match saved to database | ✅ Complete |
| Player name **cannot** be changed after first registration | ✅ By design |

---

### 3. Chat System

| Feature | Status |
|---------|--------|
| **Private (direct) chat** — message any friend by UID | ✅ Complete |
| **Group chat** — broadcast to all connected players | ✅ Complete |
| Chat history persisted in database and loaded on reconnect | ✅ Complete |

---

### 4. Backend / Database (MySQL)

| Feature | Status |
|---------|--------|
| `Players` table — UID, display name, score, wins, losses, draws | ✅ Complete |
| `Chats` table — private and group messages with timestamps | ✅ Complete |
| `GameHistory` table — full match record (players, moves, board snapshot, times) | ✅ Complete |
| `vw_leaderboard` view — top players by score | ✅ Complete |
| `vw_recent_games` view — last 100 completed games | ✅ Complete |

---

## 🚧 Not Yet Implemented (Coming Soon)

| Feature | Status |
|---------|--------|
| **Random Match** (auto-matchmaking) | 🚧 Coming soon |

---

## Tech Stack

1. C# .NET Framework 4.7.2
2. WinForms (client) + Console (server) running in parallel
3. Libraries: **Dapper**, **MySql.Data**, **System.Text.Json**
4. Author: **Miu2k3 — Minh Vương**
