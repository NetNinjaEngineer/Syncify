using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Attributes;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Conversations.Queries.GetConversationMessages;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;
using Syncify.Application.Helpers;

namespace Syncify.Api.Controllers;
[Route("api/messages")]
public class MessagesController(IMediator mediator) : ApiBaseController(mediator)
{
    [Consumes("multipart/form-data")]
    [HttpPost("send-private-message")]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<MessageDto>>> SendPrivateMessageAsync(
        [FromForm] SendPrivateMessageCommand command,
        [FromForm] IEnumerable<IFormFile> attachments)
    {
        command.Attachments = attachments;
        return CustomResult(await Mediator.Send(command));
    }

    [Guard(roles: [AppConstants.Roles.User])]
    [Consumes("multipart/form-data")]
    [HttpPost("loggedUser/send-private-message")]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<MessageDto>>> SendPrivateMessageByLoggedUserAsync(
        [FromForm] SendPrivateMessageByCurrentUserCommand command,
        [FromForm] IEnumerable<IFormFile> attachments)
    {
        command.Attachments = attachments;
        return CustomResult(await Mediator.Send(command));
    }

    [HttpGet("get-conversation-messages")]
    [ProducesResponseType(typeof(Result<ConversationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<ConversationDto>>> GetConversationMessagesAsync([FromQuery] Guid conversationId)
    {
        var conversationQuery = new GetConversationMessagesQuery() { ConversationId = conversationId };
        return CustomResult(await Mediator.Send(conversationQuery));
    }
}
