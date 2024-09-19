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
                // Truy vấn dữ liệu, lưu cột Friends dưới dạng chuỗi
                var sql = "SELECT ID, Fullname, UID, Friends AS FriendsJson, Score FROM Players WHERE UID = @UID";
                var player = connection.QueryFirstOrDefault<Player>(sql, new { UID = uid });

                if (player != null && !string.IsNullOrEmpty(player.FriendsJson))
                {
                    try
                    {
                        // Chuyển đổi chuỗi JSON thành List<Friend>
                        player.Friends = JsonSerializer.Deserialize<List<Friend>>(player.FriendsJson);
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"Error deserializing Friends JSON: {ex.Message}");
                        player.Friends = new List<Friend>(); // Gán danh sách trống nếu có lỗi
                    }
                }

                return player;
            }
        }

        public IList<Player> GetAllPlayers()
        {
            using (var connection = Connect())
            {
                var sql = "SELECT * FROM Players";
                return connection.Query<Player>(sql).ToList();
            }
        }

        public bool AddPlayer(Player player)
        {
            using (var connection = Connect())
            {
                var query = "INSERT INTO Players (Fullname, UID, Friends, Score) VALUES (@Fullname, @UID, @Friends, @Score)";

                var param = new
                {
                    Fullname = player.Fullname,
                    UID = player.UID,
                    Friends = JsonSerializer.Serialize(player.Friends),
                    Score = player.Score,

                };
                
                return connection.Execute(query, param) > 0;
            }
        }

        public bool UpdatePlayer(Player player)
        {
            using (var connection = Connect())
            {
                var sql = @"
                UPDATE Players
                SET Fullname = @Fullname, 
                    Friends = @Friends,
                    Score = @Score
                WHERE UID = @UID";

                var param = new
                {
                    Fullname = player.Fullname,
                    UID = player.UID,
                    Friends = JsonSerializer.Serialize(player.Friends),
                    Score = player.Score,

                };

                return connection.Execute(sql, param) > 0;
            }
        }

        public bool DeletePlayer(string uid)
        {
            using (var connection = Connect())
            {
                var sql = "DELETE FROM Players WHERE UID = @UID";
                return connection.Execute(sql, new { UID = uid }) > 0;
            }
        }
    }
}
