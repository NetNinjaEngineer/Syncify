using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Controllers.Base;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Features.FriendRequests.Commands.SendFriendRequest;

namespace Syncify.Api.Controllers;
[Route("api/friendRequests")]
public class FriendshipRequestsController(IMediator mediator) : ApiBaseController(mediator)
{
    [HttpPost("send")]
    public async Task<ActionResult<Result<FriendshipResponseDto>>> SendFriendshipRequestAsync(SendFriendshipRequestDto request)
        => CustomResult(await Mediator.Send(new SendFriendRequestCommand() { SendFriendshipRequest = request }));
}
