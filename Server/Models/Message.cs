using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Models
{
    public class Message
    {
        public string MessageType { get; set; } // Loại dữ liệu
        public object Data { get; set; } // Dữ liệu thực tế
    }
    public class MessageRequest
    {
        public string FriendUID { get; set; }
        public DateTime LastMessageTime { get; set; }
    }
}
