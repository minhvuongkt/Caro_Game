using Server.Models;
using System;
using System.Collections.Generic;

namespace Server.Interfaces
{
    public interface IChatDAL
    {
        bool AddChat(Chat chat);

        Chat GetChatById(int chatId);

        IList<Chat> GetChatsBetweenUsers(string user1UID, string user2UID);

        /// <summary>Returns private messages between two users sent after <paramref name="since"/>.</summary>
        IList<Chat> GetChatsBetweenUsersSince(string user1UID, string user2UID, DateTime since);

        bool UpdateChat(Chat chat);

        bool DeleteChat(int chatId);

        IList<Chat> GetChatsByGroup();

        IList<Chat> GetChatsByGroupSince(DateTime since);

        IList<Chat> GetChatsByUser(string userUID);

        bool DeleteChatsByUser(string userUID);
    }
}
