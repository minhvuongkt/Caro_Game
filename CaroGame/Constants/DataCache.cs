using Client.Models;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Client.Constants
{
    public class DataCache
    {
        public static PlayerInfo Player { get; set; }
        // Cache lưu trữ tin nhắn với key = uid friend, value = List<Chat>
        public static Dictionary<string, List<Chat>> chatCache { get; set; } = new Dictionary<string, List<Chat>>();
        // Cache group chat
        public static List<Chat> groupChatCache { get; set; } = new List<Chat>();
        // Client kết nối đến server bằng socket
        public static Socket client { get; set; }

        // Room / game state
        public static List<Room> Rooms { get; set; } = new List<Room>();
        public static Room CurrentRoom { get; set; }
        public static bool IsSpectating { get; set; }
    }
}
