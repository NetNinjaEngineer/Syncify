using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Features.FriendRequests.Commands.AcceptFriendRequest;
using Syncify.Application.Features.FriendRequests.Commands.SendFriendRequest;

namespace Syncify.Api.Controllers;
[Route("api/friendRequests")]
public class FriendshipRequestsController(IMediator mediator) : ApiBaseController(mediator)
{
    [HttpPost("send")]
    public async Task<ActionResult<Result<FriendshipResponseDto>>> SendFriendshipRequestAsync(SendFriendshipRequestDto request)
        => CustomResult(await Mediator.Send(new SendFriendRequestCommand() { SendFriendshipRequest = request }));

    [HttpPost("{friendRequestId}/accept")]
    public async Task<ActionResult<Result<FriendshipResponseDto>>> AcceptFriendRequestAsync(
        Guid friendRequestId,
        string userId)
    {
        var request = new AcceptFriendRequestDto(userId, friendRequestId);
        return CustomResult(await Mediator.Send(new AcceptFriendRequestCommand { AcceptFriendRequest = request }));
    }
}
