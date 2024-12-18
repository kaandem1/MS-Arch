using ChatMS.Models.DTO;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatMS
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> _connectedUsers = new();
        private static string _activeConversation = null;
        private static readonly ConcurrentDictionary<string, int> _lastSeenIndexes = new();


        private async Task BroadcastAdminConnectionStatus(bool isConnected)
        {
            await Clients.All.SendAsync("AdminConnectionStatus", isConnected);
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = Context.User?.FindFirstValue(ClaimTypes.Name);
            var userRole = Context.User?.FindFirstValue(ClaimTypes.Role);

            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(userName))
            {
                var roleKey = userRole == "Admin" ? "admin" : userId;
                _connectedUsers[roleKey] = Context.ConnectionId;

                Console.WriteLine($"User {userName} (Role: {userRole}) connected with ConnectionId {Context.ConnectionId}");

                await NotifyAdminOfUserUpdate(userId, userName, true);

                if (roleKey == "admin")
                {
                    await BroadcastAdminConnectionStatus(true);
                }
            }
            else
            {
                Console.WriteLine("Connection failed");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var userName = Context.User?.FindFirstValue(ClaimTypes.Name);
            var userRole = Context.User?.FindFirstValue(ClaimTypes.Role);

            if (!string.IsNullOrEmpty(userId))
            {
                var roleKey = userRole == "Admin" ? "admin" : userId;

                _connectedUsers.TryRemove(roleKey, out _);

                Console.WriteLine($"User {userName} (Role: {userRole}) disconnected.");

                if (_activeConversation == userId || roleKey == "admin")
                {
                    Console.WriteLine("Ending active conversation due to disconnection.");
                    _activeConversation = null;

                    await Clients.All.SendAsync("EndConversation", userId);
                }

                await NotifyAdminOfUserUpdate(userId, userName, false);
                if (roleKey == "admin")
                {
                    await BroadcastAdminConnectionStatus(false);
                }
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(MessageDTO message)
        {
            var senderName = Context.User?.FindFirstValue(ClaimTypes.Name);
            message.SenderName = senderName;

            if (message.Receiver == "admin")
            {
                if (_activeConversation == null || _activeConversation == message.Sender)
                {
                    _activeConversation = message.Sender;
                }
                else
                {
                    Console.WriteLine($"User {message.Sender} attempted to message admin while busy.");
                    return;
                }
            }

            if (_connectedUsers.TryGetValue(message.Receiver, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
            }
            else
            {
                Console.WriteLine($"Receiver {message.Receiver} not connected.");
            }
        }


        public async Task NotifyTyping(string receiver, bool isTyping)
        {
            if (_connectedUsers.TryGetValue(receiver, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("TypingNotification", Context.User?.FindFirstValue(ClaimTypes.Name), isTyping);
            }
        }

        private async Task NotifyAdminOfUserUpdate(string userId, string userName, bool isConnected)
        {
            if (_connectedUsers.TryGetValue("admin", out var adminConnectionId))
            {
                var update = new
                {
                    UserId = userId,
                    UserName = userName,
                    IsConnected = isConnected
                };

                await Clients.Client(adminConnectionId).SendAsync("UserUpdate", update);

                var connectedUsers = _connectedUsers
                    .Where(u => u.Key != "admin")
                    .Select(u => new { UserId = u.Key, UserName = u.Key })
                    .ToList();

                await Clients.Client(adminConnectionId).SendAsync("ConnectedUsersList", connectedUsers);
            }
        }
        public async Task MarkMessagesAsSeen(string sender, int lastSeenIndex, string seenBy)
        {
            var receiver = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var seenByName = Context.User?.FindFirstValue(ClaimTypes.Name);

            if (!string.IsNullOrEmpty(receiver) && !string.IsNullOrEmpty(seenBy))
            {
                _lastSeenIndexes[seenBy] = lastSeenIndex;

                var seenPayload = new
                {
                    LastSeenIndex = lastSeenIndex,
                    SeenBy = seenBy,
                    SeenByName = seenByName
                };

                if (_connectedUsers.TryGetValue(sender, out var senderConnectionId))
                {
                    await Clients.Client(senderConnectionId).SendAsync("MessagesSeen", seenPayload);
                }

                if (_connectedUsers.TryGetValue("admin", out var adminConnectionId))
                {
                    await Clients.Client(adminConnectionId).SendAsync("MessagesSeen", seenPayload);
                }
            }
        }

        public async Task EndConversation()
        {
            var userRole = Context.User?.FindFirstValue(ClaimTypes.Role);

            if (userRole == "Admin" || Context.User?.FindFirstValue(ClaimTypes.NameIdentifier) == _activeConversation)
            {
                Console.WriteLine("Conversation ended manually.");
                _activeConversation = null;

                await Clients.All.SendAsync("EndConversation", Context.User?.FindFirstValue(ClaimTypes.NameIdentifier));
            }
        }


    }
}
