using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Attributes;
using Syncify.Application.Bases;
using Syncify.Application.Features.Conversations.Commands.StartConversation;
using Syncify.Application.Helpers;

namespace Syncify.Api.Controllers;
[Route("api/conversations")]
public class ConversationsController(IMediator mediator) : ApiBaseController(mediator)
{
    [Guard(roles: [AppConstants.Roles.User])]
    [HttpPost("start")]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<string>>> StartConversationAsync(StartConversationCommand command)
        => CustomResult(await Mediator.Send(command));
}
