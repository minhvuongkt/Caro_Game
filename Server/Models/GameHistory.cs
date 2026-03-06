using System;

namespace Server.Models
{
    public class GameHistory
    {
        public int      ID            { get; set; }
        public string   RoomID        { get; set; }
        public GameType GameType      { get; set; }
        public string   Player1UID    { get; set; }
        public string   Player2UID    { get; set; }
        public string   Team1AllyUID  { get; set; }
        public string   Team2AllyUID  { get; set; }
        public string   WinnerUID     { get; set; }
        public int      MoveCount     { get; set; }
        public DateTime StartTime     { get; set; }
        public DateTime? EndTime      { get; set; }
        public string   BoardSnapshot { get; set; }
    }
}
