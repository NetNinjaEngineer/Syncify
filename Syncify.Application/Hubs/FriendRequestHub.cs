using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Syncify.Application.Features.FriendRequests.Commands.CurrentUserSendFriendRequest;
using Syncify.Application.Hubs.Interfaces;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Hubs;
[Authorize]
public sealed class FriendRequestHub(
    IFriendshipService friendshipService) : Hub<IFriendRequestClient>
{
    public async Task SendFriendRequest(string friendId)
    {
        var result = await friendshipService.SendFriendRequestCurrentUserAsync(
            CurrentUserSendFriendRequestCommand.Get(friendId));

        if (result.IsSuccess)
            await Clients.User(friendId).ReceiveFriendRequest($"{result.Value.Requester} sent you a friend request at {result.Value.CreatedAt}");
    }
}
