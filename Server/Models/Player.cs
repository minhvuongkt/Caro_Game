using Server.Models;
using System.Collections.Generic;

namespace Server.Models
{
    public class Player
    {
        public int ID { get; set; } = 0;
        public string Fullname { get; set; } = string.Empty;
        public string UID { get; set; } = string.Empty;
        public List<Friend> Friends { get; set; } = new List<Friend>();
        public int Score { get; set; } = 1000;
        // Thuộc tính tạm để lưu chuỗi JSON từ cơ sở dữ liệu
        public string FriendsJson { get; set; }
    }
}
