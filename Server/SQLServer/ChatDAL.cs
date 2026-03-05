using Dapper;
using Server.Interfaces;
using Server.Models;
using System;
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
                var sql = @"INSERT INTO Chats (SenderUID, ReceiverUID, Message, Time, IsGroupChat)
                            VALUES (@SenderUID, @ReceiverUID, @Message, @Time, @IsGroupChat)";
                return connection.Execute(sql, chat) > 0;
            }
        }

        public Chat GetChatById(int chatId)
        {
            using (var connection = Connect())
            {
                return connection.QueryFirstOrDefault<Chat>(
                    "SELECT * FROM Chats WHERE ID = @ChatId", new { ChatId = chatId });
            }
        }

        public IList<Chat> GetChatsBetweenUsers(string user1UID, string user2UID)
        {
            using (var connection = Connect())
            {
                var sql = @"
                SELECT * FROM Chats
                WHERE  IsGroupChat = 0
                  AND ((SenderUID = @U1 AND ReceiverUID = @U2)
                    OR (SenderUID = @U2 AND ReceiverUID = @U1))
                ORDER  BY Time ASC";
                return connection.Query<Chat>(sql, new { U1 = user1UID, U2 = user2UID }).ToList();
            }
        }

        public IList<Chat> GetChatsBetweenUsersSince(string user1UID, string user2UID, DateTime since)
        {
            using (var connection = Connect())
            {
                var sql = @"
                SELECT * FROM Chats
                WHERE  IsGroupChat = 0
                  AND ((SenderUID = @U1 AND ReceiverUID = @U2)
                    OR (SenderUID = @U2 AND ReceiverUID = @U1))
                  AND  Time > @Since
                ORDER  BY Time ASC";
                return connection.Query<Chat>(sql,
                    new { U1 = user1UID, U2 = user2UID, Since = since }).ToList();
            }
        }

        public bool UpdateChat(Chat chat)
        {
            using (var connection = Connect())
            {
                var sql = @"
                UPDATE Chats
                SET  SenderUID   = @SenderUID,
                     ReceiverUID = @ReceiverUID,
                     Message     = @Message,
                     Time        = @Time,
                     IsGroupChat = @IsGroupChat
                WHERE ID = @ID";
                return connection.Execute(sql, chat) > 0;
            }
        }

        public bool DeleteChat(int chatId)
        {
            using (var connection = Connect())
            {
                return connection.Execute(
                    "DELETE FROM Chats WHERE ID = @ChatId", new { ChatId = chatId }) > 0;
            }
        }

        public IList<Chat> GetChatsByGroup()
        {
            using (var connection = Connect())
            {
                return connection.Query<Chat>(
                    "SELECT * FROM Chats WHERE IsGroupChat = 1 ORDER BY Time ASC").ToList();
            }
        }

        public IList<Chat> GetChatsByGroupSince(DateTime since)
        {
            using (var connection = Connect())
            {
                return connection.Query<Chat>(
                    "SELECT * FROM Chats WHERE IsGroupChat = 1 AND Time > @Since ORDER BY Time ASC",
                    new { Since = since }).ToList();
            }
        }

        public IList<Chat> GetChatsByUser(string userUID)
        {
            using (var connection = Connect())
            {
                return connection.Query<Chat>(
                    "SELECT * FROM Chats WHERE SenderUID = @UID OR ReceiverUID = @UID ORDER BY Time ASC",
                    new { UID = userUID }).ToList();
            }
        }

        public bool DeleteChatsByUser(string userUID)
        {
            using (var connection = Connect())
            {
                return connection.Execute(
                    "DELETE FROM Chats WHERE SenderUID = @UID OR ReceiverUID = @UID",
                    new { UID = userUID }) > 0;
            }
        }
    }
}
