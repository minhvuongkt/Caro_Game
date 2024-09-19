using MySql.Data.MySqlClient;
using Server.Constants;
using System;
using Microsoft.Extensions.Configuration;

namespace Server.SQLServer
{
    public abstract class _BaseDAL
    {
        protected MySqlConnectionStringBuilder _stringBuilder;

        protected void _Builder()
        {
            _stringBuilder = new MySqlConnectionStringBuilder();
            _stringBuilder["Server"] = Driver.gI().MySqlHost;
            _stringBuilder["Port"] = Driver.gI().MySqlPort;
            _stringBuilder["User Id"] = Driver.gI().MySqlUsername;
            _stringBuilder["Password"] = Driver.gI().MySqlPassword;
            _stringBuilder["charset"] = "utf8mb4";
        }
        protected _BaseDAL()
        {
            var configBuilder = new ConfigurationBuilder().SetBasePath(ServerUtils.ProjectDir("./")).AddJsonFile("config.json");
            var configurationRoot = configBuilder.Build();
            Driver.CreateManager(configurationRoot);
            _Builder();
        }
        protected MySqlConnection Connect()
        {
            var Connection = new MySqlConnection();
            try
            {
                Connection?.Close();
                _stringBuilder["Database"] = Driver.gI().MySqlTable;
                Connection = new MySqlConnection(_stringBuilder.ToString());
                Connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error ConnectToSQL At DbMiu.cs: {e.Message}\n{e.StackTrace}");
            }
            return Connection;
        }
    }
}
