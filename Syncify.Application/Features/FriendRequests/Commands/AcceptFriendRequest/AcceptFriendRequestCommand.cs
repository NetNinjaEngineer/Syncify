using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;

namespace Syncify.Application.Features.FriendRequests.Commands.AcceptFriendRequest;
public sealed class AcceptFriendRequestCommand : IRequest<Result<FriendshipResponseDto>>
{
    public AcceptFriendRequestDto AcceptFriendRequest { get; set; } = null!;
}
