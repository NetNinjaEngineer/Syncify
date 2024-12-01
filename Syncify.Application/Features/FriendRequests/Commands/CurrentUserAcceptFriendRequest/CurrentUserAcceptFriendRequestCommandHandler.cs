using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.FriendRequests.Commands.CurrentUserAcceptFriendRequest;
public sealed class CurrentUserAcceptFriendRequestCommandHandler(IFriendshipService friendshipService) :
    IRequestHandler<CurrentUserAcceptFriendRequestCommand, Result<FriendshipResponseDto>>
{
    public async Task<Result<FriendshipResponseDto>> Handle(
        CurrentUserAcceptFriendRequestCommand request, CancellationToken cancellationToken)
        => await friendshipService.CurrentUserAcceptFriendRequestAsync(request);
}
