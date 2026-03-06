using Server.Models;
using System;
using System.Collections.Generic;

namespace Server.Interfaces
{
    public interface IGameHistoryDAL
    {
        /// <summary>Inserts a new game record; returns the new auto-increment ID.</summary>
        int AddGameHistory(GameHistory game);

        /// <summary>Updates WinnerUID, MoveCount, EndTime, BoardSnapshot for a finished game.</summary>
        bool FinishGame(int id, string winnerUID, int moveCount, DateTime endTime, string boardSnapshot);

        GameHistory GetGameHistoryById(int id);

        IList<GameHistory> GetGameHistoryByPlayer(string uid);

        IList<GameHistory> GetRecentGames(int count);
    }
}
