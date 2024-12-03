using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Attributes;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Messages.Commands.DeleteMessage;
using Syncify.Application.Features.Messages.Commands.DeleteMessageInConversation;
using Syncify.Application.Features.Messages.Commands.EditMessage;
using Syncify.Application.Features.Messages.Commands.MarkMessageAsRead;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;
using Syncify.Application.Features.Messages.Queries.GetMessageById;
using Syncify.Application.Features.Messages.Queries.GetMessagesByDateRange;
using Syncify.Application.Features.Messages.Queries.GetUnreadMessages;
using Syncify.Application.Features.Messages.Queries.GetUnreadMessagesCount;
using Syncify.Application.Features.Messages.Queries.SearchMessages;
using Syncify.Application.Helpers;

namespace Syncify.Api.Controllers;
[Route("api/messages")]
public class MessagesController(IMediator mediator) : ApiBaseController(mediator)
{
    [HttpPost("send")]
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
    [HttpPost("current-user/send")]
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

    [HttpGet("details")]
    [ProducesResponseType(typeof(Result<MessageDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<MessageDto>>> GetMessageByIdAsync([FromQuery] Guid messageId)
    {
        var getMessageQuery = new GetMessageByIdQuery { MessageId = messageId };
        return CustomResult(await Mediator.Send(getMessageQuery));
    }

    [HttpPut("mark-as-read")]
    public async Task<ActionResult<Result<bool>>> MarkMessageAsReadAsync(
        [FromQuery] MarkMessageAsReadCommand command)
        => CustomResult(await Mediator.Send(command));

    [HttpPut("edit")]
    public async Task<ActionResult<Result<bool>>> EditMessageAsync(
        [FromQuery] Guid messageId,
        [FromQuery] string NewContent)
        => CustomResult(await Mediator.Send(new EditMessageCommand { MessageId = messageId, NewContent = NewContent }));

    [HttpDelete("delete")]
    public async Task<ActionResult<Result<bool>>> DeleteMessageAsync(
        [FromQuery] Guid messageId)
        => CustomResult(await Mediator.Send(new DeleteMessageCommand { MessageId = messageId }));

    [HttpGet("unread-count")]
    [ProducesResponseType(typeof(Result<int>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<int>>> GetUnreadMessagesCountAsync([FromQuery] GetUnreadMessagesCountQuery query)
        => CustomResult(await Mediator.Send(query));

    [HttpGet("by-date-range")]
    [ProducesResponseType(typeof(Result<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<IEnumerable<MessageDto>>>> GetMessagesByDateRangeAsync([FromQuery] GetMessagesByDateRangeQuery query)
        => CustomResult(await Mediator.Send(query));

    [HttpGet("unread-messages")]
    [ProducesResponseType(typeof(Result<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<IEnumerable<MessageDto>>>> GetUnreadMessagesAsync([FromQuery] GetUnreadMessagesQuery query)
        => CustomResult(await Mediator.Send(query));

    [HttpGet("search")]
    [ProducesResponseType(typeof(Result<IEnumerable<MessageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<IEnumerable<MessageDto>>>> SearchMessagesAsync([FromQuery] SearchMessagesQuery query)
        => CustomResult(await Mediator.Send(query));

    [HttpDelete("{conversationId:guid}/{messageId:guid}")]
    public async Task<ActionResult<Result<bool>>> DeleteMessageInConversationAsync(
        [FromRoute] Guid conversationId, [FromRoute] Guid messageId)
      => CustomResult(await Mediator.Send(new DeleteMessageInConversationCommand { MessageId = messageId, ConversationId = conversationId }));

}