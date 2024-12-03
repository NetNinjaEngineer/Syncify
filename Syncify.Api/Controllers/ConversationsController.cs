using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Attributes;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.Features.Conversations.Commands.StartConversation;
using Syncify.Application.Features.Conversations.Queries.GetConversationMessages;
using Syncify.Application.Features.Conversations.Queries.GetPagedConversationMessages;
using Syncify.Application.Features.Conversations.Queries.GetUserConversation;
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


    [HttpGet("messages")]
    [ProducesResponseType(typeof(Result<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ConversationDto>>> GetConversationMessagesAsync([FromQuery] Guid conversationId)
    {
        var conversationQuery = new GetConversationMessagesQuery() { ConversationId = conversationId };
        return CustomResult(await Mediator.Send(conversationQuery));
    }

    [HttpGet("messages/paged")]
    [ProducesResponseType(typeof(Result<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ConversationDto>>> GetPagedConversationMessagesAsync(
        [FromQuery] GetPagedConversationMessagesQuery query) => CustomResult(await Mediator.Send(query));


    [HttpGet("user")]
    [ProducesResponseType(typeof(Result<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ConversationDto>>> GetUserConversationAsync(
        [FromQuery] GetUserConversationQuery query) => CustomResult(await Mediator.Send(query));
}
