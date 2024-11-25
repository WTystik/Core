using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
namespace Core
{
    public abstract class WebSocketHandler
    {
    private readonly ConcurrentDictionary<string, WebSocket> _sockets = new();

    public virtual async Task OnConnected(WebSocket socket)
    {
        var socketId = Guid.NewGuid().ToString();
        _sockets.TryAdd(socketId, socket);
        await BroadcastMessageAsync("New connection established.");
    }

        public virtual async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            await BroadcastMessageAsync($"Received message: {message}");
        }

        protected async Task BroadcastMessageAsync(string message)
        {
            foreach (var pair in _sockets)
            {
                if (pair.Value.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await pair.Value.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        public virtual async Task OnDisconnected(WebSocket socket)
        {
            var item = _sockets.FirstOrDefault(p => p.Value == socket);
            if (item.Key != null)
            {
                _sockets.TryRemove(item.Key, out _);
            }
            
            await BroadcastMessageAsync("Connection closed.");
        }
    }
    public class GameWebSocketHandler : WebSocketHandler
    {
        public async Task NotifyPlayerAddedAsync(string gameName, string playerName)
        {
            var message = $"Player {playerName} has joined the game {gameName}.";
            await BroadcastMessageAsync(message);
        }

        public override async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
        {
           var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
           await BroadcastMessageAsync($"Received game message: {message}");
        }
    }
}