using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;

namespace Syncify.Application.Features.FriendRequests.Commands.CurrentUserAcceptFriendRequest;
public sealed class CurrentUserAcceptFriendRequestCommand : IRequest<Result<FriendshipResponseDto>>
{
    public required Guid FriendshipId { get; set; }
}
