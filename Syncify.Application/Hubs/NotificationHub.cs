using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Syncify.Application.Hubs.Interfaces;

namespace Syncify.Application.Hubs;

[Authorize]
public sealed class NotificationHub : Hub<INotificationClient>
{
    public async Task SendNotification(string friendId, string message)
    {
        await Clients.User(friendId).ReceiveNotification(message);
    }
}
