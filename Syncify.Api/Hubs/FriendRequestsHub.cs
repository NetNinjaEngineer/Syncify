using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Syncify.Domain.Entities.Identity;

namespace Syncify.Api.Hubs;

[Authorize]
public sealed class FriendRequestsHub
    (UserManager<ApplicationUser> userManager) : Hub
{
    public async Task SendFriendRequest(string friendId)
    {
        var requester = await userManager.FindByIdAsync(Context.UserIdentifier!);

        await Clients.User(friendId).SendAsync(
            "ReceiveFriendRequest",
            string.Concat(requester!.FirstName, " ", requester.LastName),
            Context.UserIdentifier!);
    }

    public async Task UpdateFriendRequestStatus(string requesterId, string receiverId, string status)
    {
        await Clients.User(requesterId).SendAsync("FriendRequestStatusUpdated", receiverId, status);
        await Clients.User(receiverId).SendAsync("FriendRequestStatusUpdated", requesterId, status);
    }
}
