﻿using Client.Constants;
using Client.Models;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace Client.Connection
{
    public class ConnectToServer
    {
        public ConnectToServer() => Connect();

        private IPEndPoint IP;

        public void Connect()
        {
            if(DataCache.client != null && DataCache.client.Connected) return;
            try
            {
                IP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2003);
                DataCache.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                DataCache.client.Connect(IP);
            }
            catch (Exception er)
            {
                Utils.WriteLog(er.Message);
                MessageBox.Show("Lỗi kết nối. Server có vẻ đang lỗi roài. Mí bạn chờ xíu hen!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }

        public void Close()
        {
            DataCache.client.Close();
        }

        public void Send(object obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.ToString()))
            {
                byte[] data = Serialize(obj);
                DataCache.client.Send(data);
            }
        }
        public byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj); // Chuyển đối tượng thành mảng byte
        }

        public T Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data); // Chuyển mảng byte về đối tượng kiểu T
        }
    }
}
