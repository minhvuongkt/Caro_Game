using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Message
    {
        public string MessageType { get; set; }
        public object Data { get; set; }
    }
    public class MessageRequest
    {
        public string FriendUID { get; set; }
        public DateTime LastMessageTime { get; set; }
    }
}
