using Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Constants
{
    public class DataCache
    {
        public static PlayerInfo Player { get; set; }
        // Cache lưu trữ tin nhắn với key = uid friend, value = List<Chat>
        public static Dictionary<string, List<Chat>> chatCache { get; set; } = new Dictionary<string, List<Chat>>();
        // Cache group chat
        public static List<Chat> groupChatCache { get; set; } = new List<Chat>();

    }
}
