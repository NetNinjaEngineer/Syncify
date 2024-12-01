using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Attributes;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Stories;
using Syncify.Application.Features.Stories.Commands.CreateStory;
using Syncify.Application.Helpers;

namespace Syncify.Api.Controllers;
[Route("api/stories")]
[ApiController]
public class StoriesController(IMediator mediator) : ApiBaseController(mediator)
{
    [HttpPost]
    [Guard(roles: [AppConstants.Roles.User])]
    public async Task<ActionResult<Result<StoryDto>>> CreateStoryAsync(CreateStoryCommand command)
        => CustomResult(await Mediator.Send(command));
}
