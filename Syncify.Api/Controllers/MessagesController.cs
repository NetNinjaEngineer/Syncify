using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;

namespace Syncify.Api.Controllers;
[Route("api/messages")]
[ApiController]
public class MessagesController(IMediator mediator) : ApiBaseController(mediator)
{
    [HttpPost("send-private-message")]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<MessageDto>>> SendPrivateMessageAsync(
        [FromForm] SendPrivateMessageCommand command)
        => CustomResult(await Mediator.Send(command));
}
