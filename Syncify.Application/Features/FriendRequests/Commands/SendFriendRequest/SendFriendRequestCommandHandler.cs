using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.FriendRequests.Commands.SendFriendRequest;
public sealed class SendFriendRequestCommandHandler(
    IFriendshipService friendshipService) : IRequestHandler<SendFriendRequestCommand, Result<FriendshipResponseDto>>
{
    public async Task<Result<FriendshipResponseDto>> Handle(SendFriendRequestCommand request,
        CancellationToken cancellationToken)
        => await friendshipService.SendFriendRequestAsync(request.SendFriendshipRequest);
}
