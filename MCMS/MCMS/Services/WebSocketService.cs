using System.Net.WebSockets;
using System.Text;

namespace MCMS.Services
{
    public  class WebSocketService
    {
        private readonly List<WebSocket> _sockets = new List<WebSocket>();

        public async Task HandleWebSocketConnection(WebSocket webSocket)
        {
            _sockets.Add(webSocket);

            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.CloseStatus.HasValue)
                {
                    _sockets.Remove(webSocket);
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            }
        }

        public async Task SendNotification(string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);
            var tasks = _sockets.Where(s => s.State == WebSocketState.Open).Select(s =>
                s.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None));

            await Task.WhenAll(tasks);
        }
    }
}
