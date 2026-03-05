using Server.Models;
using System.Collections.Generic;

namespace Server.Interfaces
{
    public interface IPlayerDAL
    {
        Player GetPlayerByUID(string uid);

        IList<Player> GetAllPlayers();

        /// <summary>Searches for players whose Fullname contains <paramref name="namePart"/>.</summary>
        IList<Player> FindPlayersByName(string namePart);

        bool AddPlayer(Player player);

        bool UpdatePlayer(Player player);

        /// <summary>Atomically adjusts Score, Wins, Losses, Draws after a game.</summary>
        bool UpdateScore(string uid, int scoreDelta, int winDelta, int lossDelta, int drawDelta);

        bool DeletePlayer(string uid);
    }
}
