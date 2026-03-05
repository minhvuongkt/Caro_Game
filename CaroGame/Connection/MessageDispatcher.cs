using Client.Constants;
using Client.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace Client.Connection
{
    /// <summary>
    /// Single background receive loop that routes server messages to registered handlers
    /// by MessageType. This prevents the race condition that arises when multiple threads
    /// each call Socket.Receive on the shared DataCache.client socket.
    /// </summary>
    public sealed class MessageDispatcher
    {
        private static readonly MessageDispatcher _instance = new MessageDispatcher();
        public static MessageDispatcher Instance => _instance;

        private readonly ConcurrentDictionary<string, List<Action<Message>>> _handlers
            = new ConcurrentDictionary<string, List<Action<Message>>>(StringComparer.OrdinalIgnoreCase);

        private Thread _receiveThread;

        private MessageDispatcher() { }

        public void Register(string messageType, Action<Message> handler)
        {
            _handlers.AddOrUpdate(
                messageType,
                _ => new List<Action<Message>> { handler },
                (_, list) => { lock (list) { list.Add(handler); } return list; });
        }

        public void Unregister(string messageType, Action<Message> handler)
        {
            if (_handlers.TryGetValue(messageType, out var list))
                lock (list) { list.Remove(handler); }
        }

        public void Start()
        {
            if (_receiveThread != null && _receiveThread.IsAlive) return;
            _receiveThread = new Thread(ReceiveLoop) { IsBackground = true, Name = "MsgDispatcher" };
            _receiveThread.Start();
        }

        private void ReceiveLoop()
        {
            byte[] data = new byte[1024 * 5000];
            try
            {
                while (true)
                {
                    int received = DataCache.client.Receive(data);
                    if (received <= 0) continue;

                    byte[] valid = new byte[received];
                    Array.Copy(data, valid, received);

                    Message msg;
                    try { msg = JsonSerializer.Deserialize<Message>(valid); }
                    catch { continue; }

                    if (msg == null || string.IsNullOrEmpty(msg.MessageType)) continue;

                    if (_handlers.TryGetValue(msg.MessageType, out var handlers))
                    {
                        List<Action<Message>> snapshot;
                        lock (handlers) { snapshot = new List<Action<Message>>(handlers); }
                        foreach (var h in snapshot)
                        {
                            try { h(msg); }
                            catch (Exception ex) { Console.WriteLine($"Handler error [{msg.MessageType}]: {ex.Message}"); }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Show on UI thread safely; guard against no open forms
                try
                {
                    var mainForm = Application.OpenForms.Count > 0 ? Application.OpenForms[0] : null;
                    if (mainForm != null && !mainForm.IsDisposed)
                    {
                        mainForm.Invoke(new Action(() =>
                            MessageBox.Show($"Connection lost: {ex.Message}", "Lỗi kết nối",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning)));
                    }
                }
                catch { /* form may be closed */ }
            }
        }
    }
}
