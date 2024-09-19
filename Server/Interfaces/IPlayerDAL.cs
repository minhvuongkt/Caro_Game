using Server.Models;
using System.Collections.Generic;

namespace Server.Interfaces
{

    public interface IPlayerDAL
    {
        Player GetPlayerByUID(string uid);
        IList<Player> GetAllPlayers();
        bool AddPlayer(Player player);
        bool UpdatePlayer(Player player);
        bool DeletePlayer(string uid);
    }
}
