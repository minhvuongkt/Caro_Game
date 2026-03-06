using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Client.Connection
{
    /// <summary>
    /// WebSocket transport wrapper — the standard connection layer for internet / cloud deployment.
    /// Supports wss:// (TLS) URLs, including connections tunnelled through a Cloudflare or nginx
    /// reverse-proxy that provides DDoS protection and automatic TLS termination.
    ///
    /// Usage:
    ///   var proxy = new GameProxyClient("wss://game.minhvuong.io.vn");
    ///   proxy.OnConnected    += () => { /* send initial packets */ };
    ///   proxy.OnDataReceived += data => { /* parse game packets */ };
    ///   proxy.OnError        += ex => { /* handle error */ };
    ///   await proxy.ConnectAsync();
    ///
    /// Migration from raw TCP (ConnectToServer / DataCache.client):
    ///   OLD: DataCache.client.Send(bytes);
    ///   NEW: await proxy.SendAsync(bytes);
    ///
    ///   OLD: DataCache.client.Receive(buffer);
    ///   NEW: handled automatically via OnDataReceived event
    /// </summary>
    public sealed class GameProxyClient : IDisposable
    {
        private ClientWebSocket _ws;
        private readonly string _serverUrl;
        private CancellationTokenSource _cts;

        // ── Public events ─────────────────────────────────────────────────────

        /// <summary>Raised on the receive thread when the WebSocket handshake succeeds.</summary>
        public event Action OnConnected;

        /// <summary>Raised on the receive thread when the connection closes (graceful or error).</summary>
        public event Action OnDisconnected;

        /// <summary>Raised on the receive thread for every complete binary or text frame received.</summary>
        public event Action<byte[]> OnDataReceived;

        /// <summary>Raised on the receive thread when a network or protocol error occurs.</summary>
        public event Action<Exception> OnError;

        // ── Properties ────────────────────────────────────────────────────────

        /// <summary>Returns <c>true</c> while the underlying WebSocket is in the Open state.</summary>
        public bool IsConnected => _ws != null && _ws.State == WebSocketState.Open;

        // ── Constructor ───────────────────────────────────────────────────────

        /// <param name="serverUrl">
        /// Full WebSocket URL, e.g. <c>"wss://game.minhvuong.io.vn"</c> (TLS)
        /// or <c>"ws://127.0.0.1:2003"</c> (plain, LAN / local testing).
        /// </param>
        public GameProxyClient(string serverUrl)
        {
            if (string.IsNullOrWhiteSpace(serverUrl))
                throw new ArgumentNullException(nameof(serverUrl));
            _serverUrl = serverUrl;
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────

        /// <summary>
        /// Asynchronously connects to the server and starts the background receive loop.
        /// </summary>
        public async Task ConnectAsync()
        {
            _ws  = new ClientWebSocket();
            _cts = new CancellationTokenSource();
            try
            {
                await _ws.ConnectAsync(new Uri(_serverUrl), _cts.Token).ConfigureAwait(false);
                OnConnected?.Invoke();
                // Start the receive loop without blocking the caller
                _ = ReceiveLoopAsync();
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
        }

        /// <summary>Sends a raw byte array to the server as a single binary WebSocket frame.</summary>
        public async Task SendAsync(byte[] data)
        {
            if (data == null || !IsConnected) return;
            try
            {
                await _ws.SendAsync(new ArraySegment<byte>(data),
                    WebSocketMessageType.Binary, endOfMessage: true,
                    cancellationToken: _cts.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                OnError?.Invoke(ex);
            }
        }

        /// <summary>Gracefully closes the WebSocket connection.</summary>
        public async Task DisconnectAsync()
        {
            if (_ws == null) return;
            try
            {
                if (_ws.State == WebSocketState.Open)
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure,
                        "Client disconnect", CancellationToken.None).ConfigureAwait(false);
            }
            catch { /* ignore close errors */ }
            finally { _cts?.Cancel(); }
        }

        // ── Internal receive loop ─────────────────────────────────────────────

        private async Task ReceiveLoopAsync()
        {
            // Use a 64 KB receive buffer; expand for large board snapshots if needed
            var buffer = new byte[1024 * 64];
            try
            {
                while (_ws.State == WebSocketState.Open && !_cts.IsCancellationRequested)
                {
                    var result = await _ws.ReceiveAsync(
                        new ArraySegment<byte>(buffer), _cts.Token).ConfigureAwait(false);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure,
                            string.Empty, CancellationToken.None).ConfigureAwait(false);
                        break;
                    }

                    // Copy exactly the received bytes into a fresh array before raising the event
                    var data = new byte[result.Count];
                    Array.Copy(buffer, data, result.Count);
                    OnDataReceived?.Invoke(data);
                }
            }
            catch (OperationCanceledException) { /* normal shutdown */ }
            catch (Exception ex) { OnError?.Invoke(ex); }
            finally { OnDisconnected?.Invoke(); }
        }

        // ── IDisposable ───────────────────────────────────────────────────────

        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _ws?.Dispose();
        }
    }
}
