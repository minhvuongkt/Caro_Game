using System.Collections.Generic;

namespace Server.Models
{
    public enum GameType { Caro = 0, Go = 1, TwoVsTwo = 2 }
    public enum RoomStatus { Waiting = 0, Playing = 1, Finished = 2 }

    public class Room
    {
        public string RoomID { get; set; }
        public string Name { get; set; }
        public string HostUID { get; set; }

        // 1v1 players
        public string Player1UID { get; set; }
        public string Player2UID { get; set; }

        // Extra slots for 2v2: Team1 = Player1UID + Team1AllyUID, Team2 = Player2UID + Team2AllyUID
        public string Team1AllyUID { get; set; }
        public string Team2AllyUID { get; set; }

        public List<string> SpectatorUIDs { get; set; } = new List<string>();

        public GameType GameType { get; set; }
        public RoomStatus Status { get; set; }

        // Board stored as flat array indexed [row * BoardSize + col]
        // Values: "" = empty, "X" = player1/team1, "O" = player2/team2
        public int BoardSize { get; set; }
        public string[] Board { get; set; }

        public string CurrentTurnUID { get; set; }
        public string WinnerUID { get; set; }
        public int MoveCount { get; set; }
    }
}
