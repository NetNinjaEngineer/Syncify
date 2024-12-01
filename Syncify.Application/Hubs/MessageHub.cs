using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Hubs.Interfaces;

namespace Syncify.Application.Hubs;

[Authorize]
public sealed class MessageHub : Hub<IMessageClient>
{
    public async Task SendPrivateMessage(string receiverId, MessageDto message)
    {
        await Clients.User(receiverId).ReceivePrivateMessage(message);
    }
}
