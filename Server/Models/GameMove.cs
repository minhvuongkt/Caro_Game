namespace Server.Models
{
    public class GameMove
    {
        public string RoomID { get; set; }
        public string PlayerUID { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
    }
}
