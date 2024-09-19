using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Models
{
    public class Chat
    {
        public int ID { get; set; }
        public string SenderUID { get; set; }
        public string ReceiverUID { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public bool IsGroupChat { get; set; }
    }
}
