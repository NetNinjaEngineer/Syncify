using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;

namespace Syncify.Application.Features.FriendRequests.Commands.CurrentUserSendFriendRequest;
public sealed class CurrentUserSendFriendRequestCommand : IRequest<Result<FriendshipResponseDto>>
{
    public string FriendId { get; set; } = null!;

    public static CurrentUserSendFriendRequestCommand Get(string friendId) => new() { FriendId = friendId };
}
