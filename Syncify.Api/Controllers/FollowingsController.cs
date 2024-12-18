﻿using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Attributes;
using Syncify.Api.Base;
using Syncify.Application.Bases;
using Syncify.Application.Features.Followings.Commands.FollowUser;
using Syncify.Application.Features.Followings.Commands.UnFollowUser;

namespace Syncify.Api.Controllers;
[Guard]
[Route("api/followings")]
[ApiController]
public class FollowingsController(IMediator mediator) : ApiBaseController(mediator)
{
    [HttpPost("follow")]
    public async Task<ActionResult<Result<bool>>> FollowUserAsync(
        [FromQuery] string followerId,
        [FromQuery] string followedId)
        => CustomResult(
            await Mediator.Send(new FollowUserCommand { FollowerId = followerId, FollowedId = followedId }));

    [HttpPost("unfollow")]
    public async Task<ActionResult<Result<bool>>> UnFollowUserAsync(
        [FromQuery] string followerId,
        [FromQuery] string followedId)
        => CustomResult(
            await Mediator.Send(new UnFollowUserCommand { FollowerId = followerId, FollowedId = followedId }));
}
