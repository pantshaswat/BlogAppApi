using Microsoft.AspNetCore.Mvc;
using blogAppApi.Models;
using blogAppApi.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace blogApp.Data
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebSocketController : ControllerBase
    {
        private readonly AppDbContext _context;
        public WebSocketController(AppDbContext context)
        {
            _context = context;
        }

        public static ConcurrentDictionary<int, WebSocket> _sockets = new ConcurrentDictionary<int, WebSocket>();
[HttpGet]
        public async Task Connect(int userId)
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                Console.WriteLine("WebSocket Request");
                var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSocket Connected with userId: " + userId);
                _sockets.TryAdd(userId, webSocket);

                try
                {
                
                    await ReceiveMessage(webSocket, userId); // Listen for incoming messages
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error handling WebSocket connection: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Bad Request");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
[HttpPost]
[ApiExplorerSettings(IgnoreApi = true)]
        public async Task ReceiveMessage(WebSocket webSocket, int senderUserId)
{
    var buffer = new byte[1024 * 4]; // 4KB buffer
    while (webSocket.State == WebSocketState.Open)
    {
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Text)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"Received message from userId {senderUserId}: {message}");

            // Parse message to extract destination user ID and message content
            var messageParts = message.Split('|'); // Assuming '|' separates userId and message content
            if (messageParts.Length == 2 && int.TryParse(messageParts[0], out int receiverUserId))
            {
                await SendMessageToUser(receiverUserId, messageParts[1]); // Send message to the specified user
            }
            else
            {
                Console.WriteLine($"Invalid message format received from userId {senderUserId}: {message}");
            }
        }
        else if (result.MessageType == WebSocketMessageType.Close)
        {
            Console.WriteLine($"WebSocket connection closed for userId {senderUserId}");
            break;
        }
    }
    _sockets.TryRemove(senderUserId, out _); // Remove WebSocket from dictionary when connection is closed
    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "WebSocket connection closed", CancellationToken.None);
}

private async Task SendMessageToUser(int userId, string message)
{
    if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
    {
        var buffer = Encoding.UTF8.GetBytes(message);
        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    else
    {
        Console.WriteLine($"User with userId {userId} is not connected or WebSocket is not open.");
    }
}

        private async Task SendNotification(int userId, string message)
        {
            if (_sockets.TryGetValue(userId, out var socket) && socket.State == WebSocketState.Open)
            {
                var buffer = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}
