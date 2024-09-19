using Server.Models;
using System.Collections.Generic;
namespace Server.Interfaces
{
    public interface IChatDAL
    {

        // Thêm một tin nhắn vào cơ sở dữ liệu
        bool AddChat(Chat chat);

        // Lấy tin nhắn theo ID
        Chat GetChatById(int chatId);

        // Lấy tất cả tin nhắn giữa hai người dùng
        IList<Chat> GetChatsBetweenUsers(string user1UID, string user2UID);

        // Cập nhật một tin nhắn
        bool UpdateChat(Chat chat);

        // Xóa một tin nhắn
        bool DeleteChat(int chatId);

        // Lấy tất cả tin nhắn trong một nhóm
        IList<Chat> GetChatsByGroup(string groupUID);

        // Lấy tất cả tin nhắn của một người dùng
        IList<Chat> GetChatsByUser(string userUID);

        // Xóa tất cả tin nhắn của một người dùng
        bool DeleteChatsByUser(string userUID);
    }
}
