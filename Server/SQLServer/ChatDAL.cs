using Dapper;
using Server.Interfaces;
using Server.Models;
using System.Collections.Generic;
using System.Linq;

namespace Server.SQLServer
{
    public class ChatDAL : _BaseDAL, IChatDAL
    {
        public ChatDAL() : base() { }

        public bool AddChat(Chat chat)
        {
            using (var connection = Connect())
            {
                var sql = "INSERT INTO Chats (SenderUID, ReceiverUID, Message, Time, IsGroupChat) VALUES (@SenderUID, @ReceiverUID, @Message, @Time, @IsGroupChat)";
                return connection.Execute(sql, chat) > 0;
            }
        }

        public Chat GetChatById(int chatId)
        {
            using (var connection = Connect())
            {
                var sql = "SELECT * FROM Chats WHERE ID = @ChatId";
                return connection.QueryFirstOrDefault<Chat>(sql, new { ChatId = chatId });
            }
        }

        public IList<Chat> GetChatsBetweenUsers(string user1UID, string user2UID)
        {
            using (var connection = Connect())
            {
                var sql = @"
                SELECT * FROM Chats 
                WHERE (SenderUID = @User1UID AND ReceiverUID = @User2UID) 
                OR (SenderUID = @User2UID AND ReceiverUID = @User1UID)";
                return connection.Query<Chat>(sql, new { User1UID = user1UID, User2UID = user2UID }).ToList();
            }
        }

        public bool UpdateChat(Chat chat)
        {
            using (var connection = Connect())
            {
                var sql = @"
                UPDATE Chats 
                SET SenderUID = @SenderUID, 
                    ReceiverUID = @ReceiverUID, 
                    Message = @Message, 
                    Time = @Time, 
                    IsGroupChat = @IsGroupChat 
                WHERE ID = @ID";

                return connection.Execute(sql, chat) > 0;
            }
        }

        public bool DeleteChat(int chatId)
        {
            using (var connection = Connect())
            {
                var sql = "DELETE FROM Chats WHERE ID = @ChatId";
                return connection.Execute(sql, new { ChatId = chatId }) > 0;
            }
        }

        public IList<Chat> GetChatsByGroup(string groupUID)
        {
            using (var connection = Connect())
            {
                var sql = "SELECT * FROM Chats WHERE ReceiverUID = @GroupUID AND IsGroupChat = 1";
                return connection.Query<Chat>(sql, new { GroupUID = groupUID }).ToList();
            }
        }

        public IList<Chat> GetChatsByUser(string userUID)
        {
            using (var connection = Connect())
            {
                var sql = "SELECT * FROM Chats WHERE SenderUID = @UserUID OR ReceiverUID = @UserUID";
                return connection.Query<Chat>(sql, new { UserUID = userUID }).ToList();
            }
        }

        public bool DeleteChatsByUser(string userUID)
        {
            using (var connection = Connect())
            {
                var sql = "DELETE FROM Chats WHERE SenderUID = @UserUID OR ReceiverUID = @UserUID";
                return connection.Execute(sql, new { UserUID = userUID }) > 0;
            }
        }
    }
}
