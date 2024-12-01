using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;

namespace Syncify.Application.Features.FriendRequests.Commands.SendFriendRequest;
public sealed class SendFriendRequestCommand : IRequest<Result<FriendshipResponseDto>>
{
    public SendFriendshipRequestDto SendFriendshipRequest { get; set; } = null!;
}
