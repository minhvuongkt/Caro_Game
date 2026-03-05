using Dapper;
using MySql.Data.MySqlClient;
using Server.Interfaces;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Server.SQLServer
{
    public class PlayerDAL : _BaseDAL, IPlayerDAL
    {
        public PlayerDAL() : base() { }

        public Player GetPlayerByUID(string uid)
        {
            using (var connection = Connect())
            {
                var sql = @"SELECT ID, Fullname, UID, Friends AS FriendsJson,
                                   Score, Wins, Losses, Draws
                            FROM   Players
                            WHERE  UID = @UID";
                var player = connection.QueryFirstOrDefault<Player>(sql, new { UID = uid });
                DeserializeFriends(player);
                return player;
            }
        }

        public IList<Player> GetAllPlayers()
        {
            using (var connection = Connect())
            {
                var players = connection.Query<Player>(
                    "SELECT ID, Fullname, UID, Friends AS FriendsJson, Score, Wins, Losses, Draws FROM Players")
                    .ToList();
                foreach (var p in players) DeserializeFriends(p);
                return players;
            }
        }

        public IList<Player> FindPlayersByName(string namePart)
        {
            using (var connection = Connect())
            {
                var sql = @"SELECT ID, Fullname, UID, Friends AS FriendsJson, Score, Wins, Losses, Draws
                            FROM   Players
                            WHERE  Fullname LIKE @Pattern
                            LIMIT  20";
                var players = connection.Query<Player>(sql,
                    new { Pattern = $"%{namePart}%" }).ToList();
                foreach (var p in players) DeserializeFriends(p);
                return players;
            }
        }

        public bool AddPlayer(Player player)
        {
            using (var connection = Connect())
            {
                var sql = @"INSERT INTO Players (Fullname, UID, Friends, Score, Wins, Losses, Draws)
                            VALUES (@Fullname, @UID, @Friends, @Score, @Wins, @Losses, @Draws)";
                return connection.Execute(sql, new
                {
                    player.Fullname,
                    player.UID,
                    Friends = JsonSerializer.Serialize(player.Friends),
                    player.Score,
                    player.Wins,
                    player.Losses,
                    player.Draws
                }) > 0;
            }
        }

        public bool UpdatePlayer(Player player)
        {
            using (var connection = Connect())
            {
                var sql = @"UPDATE Players
                            SET  Fullname = @Fullname,
                                 Friends  = @Friends,
                                 Score    = @Score,
                                 Wins     = @Wins,
                                 Losses   = @Losses,
                                 Draws    = @Draws
                            WHERE UID = @UID";
                return connection.Execute(sql, new
                {
                    player.Fullname,
                    player.UID,
                    Friends = JsonSerializer.Serialize(player.Friends),
                    player.Score,
                    player.Wins,
                    player.Losses,
                    player.Draws
                }) > 0;
            }
        }

        public bool UpdateScore(string uid, int scoreDelta, int winDelta,
                                int lossDelta, int drawDelta)
        {
            using (var connection = Connect())
            {
                var sql = @"UPDATE Players
                            SET  Score  = GREATEST(0, Score  + @ScoreDelta),
                                 Wins   = Wins   + @WinDelta,
                                 Losses = Losses + @LossDelta,
                                 Draws  = Draws  + @DrawDelta
                            WHERE UID = @UID";
                return connection.Execute(sql, new
                {
                    UID        = uid,
                    ScoreDelta = scoreDelta,
                    WinDelta   = winDelta,
                    LossDelta  = lossDelta,
                    DrawDelta  = drawDelta
                }) > 0;
            }
        }

        public bool DeletePlayer(string uid)
        {
            using (var connection = Connect())
            {
                return connection.Execute(
                    "DELETE FROM Players WHERE UID = @UID", new { UID = uid }) > 0;
            }
        }

        // ── helper ────────────────────────────────────────────────────────────

        private static void DeserializeFriends(Player player)
        {
            if (player == null || string.IsNullOrEmpty(player.FriendsJson)) return;
            try
            {
                player.Friends = JsonSerializer.Deserialize<List<Friend>>(player.FriendsJson);
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Error deserializing Friends JSON: {ex.Message}");
                player.Friends = new List<Friend>();
            }
        }
    }
}
