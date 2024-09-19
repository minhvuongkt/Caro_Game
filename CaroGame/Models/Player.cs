using System.Collections.Generic;

namespace Client.Models
{
    enum Player
    {
        X, O
    }
    public class PlayerInfo
    {
        public int ID { get; set; } = 0;
        public string Fullname { get; set; } = string.Empty;
        public string UID { get; set; } = string.Empty;
        public List<Friend> Friends { get; set; } = new List<Friend>();
        public int Score { get; set; } = 1000;
    }
}
