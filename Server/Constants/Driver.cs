using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Server.Constants
{
    public class Driver
    {
        private readonly IConfiguration _configuration;
        private static Driver _instance;
        public string ServerIP { get; set; }
        public int ServerPort { get; set; }
        public string MySqlHost { get; set; }
        public int MySqlPort { get; set; }
        public string MySqlUsername { get; set; }
        public string MySqlPassword { get; set; }
        public string MySqlTable { get; set; }
        public static Driver gI()
        {
            return _instance;
        }
        public static void CreateManager(IConfiguration configuration)
        {
            if (_instance == null) 
                _instance = new Driver(configuration);
        }
        public Driver(IConfiguration configuration)
        {
            _configuration = configuration;
            _Init();
        }
        public void _Init()
        {
            try
            {
                ServerIP = _configuration.GetSection("server").GetSection("host").Value;
                ServerPort = int.Parse(_configuration.GetSection("server").GetSection("port").Value);
                MySqlHost = _configuration.GetSection("database").GetSection("mysql").GetSection("host").Value;
                MySqlPort = int.Parse(_configuration.GetSection("database").GetSection("mysql").GetSection("port").Value);
                MySqlUsername = _configuration.GetSection("database").GetSection("mysql").GetSection("user").Value;
                MySqlPassword = _configuration.GetSection("database").GetSection("mysql").GetSection("password").Value;
                MySqlTable = _configuration.GetSection("database").GetSection("mysql").GetSection("database").Value;
            }
            catch
            {
                ServerIP = "127.0.0.1";
                ServerPort = 3004;
                MySqlHost = "127.0.0.1";
                MySqlUsername = "root";
                MySqlPassword = "";
                MySqlPort = 3306;
                MySqlTable = "server_chat";
            }
        }
    }
}
