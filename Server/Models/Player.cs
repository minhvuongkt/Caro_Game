using Server.Models;
using System.Collections.Generic;

namespace Server.Models
{
    public class Player
    {
        public int    ID          { get; set; } = 0;
        public string Fullname    { get; set; } = string.Empty;
        public string UID         { get; set; } = string.Empty;
        public List<Friend> Friends { get; set; } = new List<Friend>();
        public int    Score       { get; set; } = 1000;
        public int    Wins        { get; set; } = 0;
        public int    Losses      { get; set; } = 0;
        public int    Draws       { get; set; } = 0;
        // Temporary property to hold raw JSON from the database
        public string FriendsJson { get; set; }
    }
}
