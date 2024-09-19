using Server.Constants;
using Server.Models;
using Server.SQLServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace Server.Handlers
{
    /*public class ServerHandler
    {
        static IPEndPoint IP { get; set; }
        static Socket server { get; set; }
        static List<Socket> clientList { get; set; }
        readonly ChatDAL messageDAL;
        readonly PlayerDAL playerDAL;
        // Cache cho tin nhắn group chat
        static List<Chat> groupChatMessages { get; set; } = new List<Chat>();

        // Danh sách các client tham gia group chat
        static List<Socket> groupChatMembers { get; set; } = new List<Socket>();

        public ServerHandler()
        {
            playerDAL = new PlayerDAL();
            messageDAL = new ChatDAL();
            KhoiTao();
        }
        public static void KhoiTao()
        {
            clientList = new List<Socket>();
            IP = new IPEndPoint(IPAddress.Parse(Driver.gI().ServerIP), Driver.gI().ServerPort);
            StartServer();
        }
        static void StartServer()
        {
            if (server == null || !server.IsBound)
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Parse(Driver.gI().ServerIP), Driver.gI().ServerPort));
                Console.WriteLine($"Server started listening on port: {Driver.gI().ServerPort}");
            }

            Thread listenThread = new Thread(() =>
            {
                try
                {
                    server.Listen(100);
                    while (true)
                    {
                        Socket client = server.Accept();
                        clientList.Add(client);
                        Console.WriteLine($"{client.RemoteEndPoint} is connected!");

                        Thread receiveThread = new Thread(() => Receive(client));
                        receiveThread.IsBackground = true;
                        receiveThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server exception: {ex.Message}");
                    Close();
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public static void Close()
        {
            try
            {
                foreach (var client in clientList)
                {
                    client.Close();
                }
                server.Close();
                Console.WriteLine("Server closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Close exception: {ex.Message}");
            }
            Environment.Exit(0);
        }

        public static void Send(Socket client, object obj)
        {
            if (client != null && obj != null)
            {
                try
                {
                    byte[] data = Serialize(obj);
                    client.Send(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Send exception: {ex.Message}");
                }
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    int receivedBytes = client.Receive(data);

                    if (receivedBytes > 0)
                    {
                        byte[] validData = new byte[receivedBytes];
                        Array.Copy(data, validData, receivedBytes);

                        var chatMessage = Deserialize<Chat>(validData);

                        if (chatMessage != null)
                        {
                            if (chatMessage.IsGroupChat)
                            {
                                // Add the client to group chat if not already added
                                if (!groupChatMembers.Contains(client))
                                {
                                    groupChatMembers.Add(client);
                                }

                                // Cache the message
                                groupChatMessages.Add(chatMessage);

                                // Broadcast the message to all group members
                                BroadcastToGroupChat(chatMessage);
                            }
                            else
                            {
                                // Handle private chat messages
                                HandlePrivateChat(chatMessage);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receive exception: {ex.Message}");
                clientList.Remove(client);
                client.Close();
            }
        }

        private static void BroadcastToGroupChat(Chat message)
        {
            foreach (var member in groupChatMembers)
            {
                try
                {
                    Send(member, message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Broadcast exception: {ex.Message}");
                }
            }
        }

        private static void HandlePrivateChat(Chat message)
        {
            // Find the recipient from the client list
            var receiver = clientList.FirstOrDefault(c => c.RemoteEndPoint.ToString() == message.ReceiverUID);

            if (receiver != null)
            {
                // Send the message to the recipient
                Send(receiver, message);
            }
            else
            {
                Console.WriteLine($"User {message.ReceiverUID} not found");
            }
        }

        static byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        static T Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        public static void RemoveClient(Socket client)
        {
            clientList.Remove(client);
            client.Disconnect(false);
            groupChatMembers.Remove(client);
        }
    }*/


    public class ServerHandler
    {
        private IPEndPoint IP { get; set; }
        private Socket server { get; set; }
        private List<Socket> clientList { get; set; }
        private readonly ChatDAL messageDAL;
        private readonly PlayerDAL playerDAL;

        // Cache cho tin nhắn group chat
        private List<Chat> groupChatMessages { get; set; } = new List<Chat>();

        // Danh sách các client tham gia group chat
        private List<Socket> groupChatMembers { get; set; } = new List<Socket>();

        public ServerHandler()
        {
            playerDAL = new PlayerDAL();
            messageDAL = new ChatDAL();
            KhoiTao();
        }

        private void KhoiTao()
        {
            clientList = new List<Socket>();
            IP = new IPEndPoint(IPAddress.Parse(Driver.gI().ServerIP), Driver.gI().ServerPort);
            StartServer();
        }

        private void StartServer()
        {
            if (server == null || !server.IsBound)
            {
                server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                server.Bind(new IPEndPoint(IPAddress.Parse(Driver.gI().ServerIP), Driver.gI().ServerPort));
                Console.WriteLine($"Server started listening on port: {Driver.gI().ServerPort}");
            }

            Thread listenThread = new Thread(() =>
            {
                try
                {
                    server.Listen(100);
                    while (true)
                    {
                        Socket client = server.Accept();
                        clientList.Add(client);
                        Console.WriteLine($"{client.RemoteEndPoint} is connected!");

                        Thread receiveThread = new Thread(() => Receive(client));
                        receiveThread.IsBackground = true;
                        receiveThread.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Server exception: {ex.Message}");
                    Close();
                }
            });

            listenThread.IsBackground = true;
            listenThread.Start();
        }

        public void Close()
        {
            try
            {
                foreach (var client in clientList)
                {
                    client.Close();
                }
                server.Close();
                Console.WriteLine("Server closed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Close exception: {ex.Message}");
            }
            Environment.Exit(0);
        }

        public void Send(Socket client, object obj)
        {
            if (client != null && obj != null)
            {
                try
                {
                    byte[] data = Serialize(obj);
                    client.Send(data);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Send exception: {ex.Message}");
                }
            }
        }

        private void Receive(Socket client)
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    int receivedBytes = client.Receive(data);

                    if (receivedBytes > 0)
                    {
                        byte[] validData = new byte[receivedBytes];
                        Array.Copy(data, validData, receivedBytes);

                        var message = Deserialize<Message>(validData);

                        if (message != null)
                        {
                            switch (message.MessageType)
                            {
                                case "Player":
                                    {
                                        var player = DeserializeFromJsonElement<Player>(message.Data);
                                        if (player != null)
                                        {
                                            if (player.ID == 0 && player.UID != "GetPlayer")
                                            {
                                                player.UID = client.RemoteEndPoint.ToString().Split(':')[0];
                                                var checkPl = playerDAL.GetPlayerByUID(player.UID);
                                                if (checkPl == null)
                                                {
                                                    if (playerDAL.AddPlayer(player))
                                                    {
                                                        var msg = new Message()
                                                        {
                                                            MessageType = "Player",
                                                            Data = player
                                                        };
                                                        Send(client, msg);
                                                        Console.WriteLine($"Create successful player UID: {client.RemoteEndPoint.ToString().Split(':')[0]}");
                                                    }
                                                }
                                                else if (checkPl != null)
                                                {
                                                    var msg = new Message()
                                                    {
                                                        MessageType = "Player",
                                                        Data = checkPl
                                                    };
                                                    Send(client, msg);
                                                }
                                                break;
                                            }
                                            else if (player.UID == "GetPlayer")
                                            {
                                                var checkPl = playerDAL.GetPlayerByUID(client.RemoteEndPoint.ToString().Split(':')[0]);
                                                if (checkPl != null)
                                                {
                                                    var msg = new Message()
                                                    {
                                                        MessageType = "Player",
                                                        Data = checkPl
                                                    };
                                                    Send(client, msg);
                                                }
                                                else
                                                {
                                                    Console.WriteLine($"Unknown player UID: {client.RemoteEndPoint.ToString().Split(':')[0]}");
                                                }
                                                break;
                                            }
                                            else if (player.ID > 0)
                                            {
                                                playerDAL.UpdatePlayer(player);
                                            }
                                        }
                                        break;
                                    }
                                case "Chat":
                                    {
                                        var chatMessage = DeserializeFromJsonElement<Chat>(message.Data);
                                        if (chatMessage.IsGroupChat)
                                        {
                                            // Add the client to group chat if not already added
                                            if (!groupChatMembers.Contains(client))
                                            {
                                                groupChatMembers.Add(client);
                                            }

                                            // Cache the message
                                            groupChatMessages.Add(chatMessage);

                                            // Save to database and broadcast the message to all group members
                                            messageDAL.AddChat(chatMessage);
                                            BroadcastToGroupChat(client, chatMessage);
                                        }
                                        else
                                        {
                                            // Handle private chat messages
                                            HandlePrivateChat(chatMessage);
                                        }
                                        break;
                                    }

                                case "MessageRequest":
                                    {
                                        var request = DeserializeFromJsonElement<MessageRequest>(message.Data);
                                        HandleMessageRequest(client, request);
                                        break;
                                    }

                                // Handle other message types here if needed

                                default:
                                    Console.WriteLine($"Unknown message type: {message.MessageType}");
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receive exception: {ex.Message}");
                clientList.Remove(client);
                client.Close();
            }
        }

        private void BroadcastToGroupChat(Socket client, Chat message)
        {
            foreach (var member in groupChatMembers)
            {
                try
                {
                    if (member != client)
                    {
                        Send(member, new Message { MessageType = "Chat", Data = message });
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Broadcast exception: {ex.Message}");
                }
            }
        }

        private void HandlePrivateChat(Chat message)
        {
            // Find the recipient from the client list
            var receiver = clientList.FirstOrDefault(c => c.RemoteEndPoint.ToString() == message.ReceiverUID);

            if (receiver != null)
            {
                // Send the message to the recipient
                Send(receiver, new Message { MessageType = "Chat", Data = message });
            }
            else
            {
                Console.WriteLine($"User {message.ReceiverUID} not found");
            }
        }

        private void HandleMessageRequest(Socket client, MessageRequest request)
        {
            if (request.FriendUID == "Group Chat")
            {
                // Return all group chat messages since LastMessageTime
                groupChatMessages = messageDAL.GetChatsByGroup().ToList();
                var newMessages = groupChatMessages.Where(m => m.Time > request.LastMessageTime).ToList();
                Send(client, new Message { MessageType = "ChatList", Data = newMessages });
            }
            else
            {
                // Return all private chat messages with the specified friend since LastMessageTime
                var newMessages = messageDAL.GetChatsBetweenUsers(request.FriendUID, "CurrentUserUID") // Replace with actual UID
                    .Where(m => m.Time > request.LastMessageTime).ToList();
                Send(client, new Message { MessageType = "ChatList", Data = newMessages });
            }
        }

        private byte[] Serialize<T>(T obj)
        {
            return JsonSerializer.SerializeToUtf8Bytes(obj);
        }

        private T Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data);
        }
        private T DeserializeFromJsonElement<T>(object data)
        {
            if (data is JsonElement jsonElement)
            {
                // Convert JsonElement to the specific object
                return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
            }
            throw new InvalidCastException("Cannot cast the object to the specified type.");
        }
        public void RemoveClient(Socket client)
        {
            clientList.Remove(client);
            client.Disconnect(false);
            groupChatMembers.Remove(client);
        }
    }


}