using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.FriendRequests.Commands.AcceptFriendRequest;
public sealed class AcceptFriendRequestCommandHandler(IFriendshipService friendshipService) :
    IRequestHandler<AcceptFriendRequestCommand, Result<FriendshipResponseDto>>
{
    public async Task<Result<FriendshipResponseDto>> Handle(
        AcceptFriendRequestCommand request,
        CancellationToken cancellationToken)
        => await friendshipService.AcceptFriendRequestAsync(request.AcceptFriendRequest);
}
