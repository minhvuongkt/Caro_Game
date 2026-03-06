using System.Collections.Generic;

namespace Client.Models
{
    public enum GameType { Caro = 0, Go = 1, TwoVsTwo = 2 }
    public enum RoomStatus { Waiting = 0, Playing = 1, Finished = 2 }

    public class Room
    {
        public string RoomID { get; set; }
        public string Name { get; set; }
        public string HostUID { get; set; }
        public string Player1UID { get; set; }
        public string Player2UID { get; set; }
        public string Team1AllyUID { get; set; }
        public string Team2AllyUID { get; set; }
        public System.Collections.Generic.List<string> SpectatorUIDs { get; set; } = new System.Collections.Generic.List<string>();
        public GameType GameType { get; set; }
        public RoomStatus Status { get; set; }
        public int BoardSize { get; set; }
        public string[] Board { get; set; }
        public string CurrentTurnUID { get; set; }
        public string WinnerUID { get; set; }
        public int MoveCount { get; set; }
    }
}
