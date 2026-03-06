using Dapper;
using Server.Interfaces;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.SQLServer
{
    public class GameHistoryDAL : _BaseDAL, IGameHistoryDAL
    {
        public GameHistoryDAL() : base() { }

        public int AddGameHistory(GameHistory game)
        {
            using (var connection = Connect())
            {
                var sql = @"
                INSERT INTO GameHistory
                    (RoomID, GameType, Player1UID, Player2UID, Team1AllyUID, Team2AllyUID,
                     WinnerUID, MoveCount, StartTime, EndTime, BoardSnapshot)
                VALUES
                    (@RoomID, @GameType, @Player1UID, @Player2UID, @Team1AllyUID, @Team2AllyUID,
                     @WinnerUID, @MoveCount, @StartTime, @EndTime, @BoardSnapshot);
                SELECT LAST_INSERT_ID();";

                return connection.ExecuteScalar<int>(sql, new
                {
                    game.RoomID,
                    GameType      = (int)game.GameType,
                    game.Player1UID,
                    game.Player2UID,
                    game.Team1AllyUID,
                    game.Team2AllyUID,
                    game.WinnerUID,
                    game.MoveCount,
                    game.StartTime,
                    game.EndTime,
                    game.BoardSnapshot
                });
            }
        }

        public bool FinishGame(int id, string winnerUID, int moveCount,
                               DateTime endTime, string boardSnapshot)
        {
            using (var connection = Connect())
            {
                var sql = @"
                UPDATE GameHistory
                SET WinnerUID     = @WinnerUID,
                    MoveCount     = @MoveCount,
                    EndTime       = @EndTime,
                    BoardSnapshot = @BoardSnapshot
                WHERE ID = @ID";

                return connection.Execute(sql, new
                {
                    ID            = id,
                    WinnerUID     = winnerUID,
                    MoveCount     = moveCount,
                    EndTime       = endTime,
                    BoardSnapshot = boardSnapshot
                }) > 0;
            }
        }

        public GameHistory GetGameHistoryById(int id)
        {
            using (var connection = Connect())
            {
                return connection.QueryFirstOrDefault<GameHistory>(
                    "SELECT * FROM GameHistory WHERE ID = @ID", new { ID = id });
            }
        }

        public IList<GameHistory> GetGameHistoryByPlayer(string uid)
        {
            using (var connection = Connect())
            {
                var sql = @"
                SELECT * FROM GameHistory
                WHERE  Player1UID = @UID OR Player2UID = @UID
                    OR Team1AllyUID = @UID OR Team2AllyUID = @UID
                ORDER  BY StartTime DESC
                LIMIT  50";
                return connection.Query<GameHistory>(sql, new { UID = uid }).ToList();
            }
        }

        public IList<GameHistory> GetRecentGames(int count)
        {
            using (var connection = Connect())
            {
                return connection.Query<GameHistory>(
                    "SELECT * FROM GameHistory ORDER BY StartTime DESC LIMIT @Count",
                    new { Count = count }).ToList();
            }
        }
    }
}
