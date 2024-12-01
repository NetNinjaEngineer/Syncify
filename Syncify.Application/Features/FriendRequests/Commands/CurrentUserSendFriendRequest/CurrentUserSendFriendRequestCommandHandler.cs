using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.FriendRequests.Commands.CurrentUserSendFriendRequest;
public sealed class CurrentUserSendFriendRequestCommandHandler(
    IFriendshipService friendshipService) : IRequestHandler<CurrentUserSendFriendRequestCommand, Result<FriendshipResponseDto>>
{
    public async Task<Result<FriendshipResponseDto>> Handle(
        CurrentUserSendFriendRequestCommand request, CancellationToken cancellationToken)
        => await friendshipService.SendFriendRequestCurrentUserAsync(request);
}
