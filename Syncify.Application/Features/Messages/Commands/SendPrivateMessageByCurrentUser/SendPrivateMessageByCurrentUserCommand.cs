using MediatR;
using Microsoft.AspNetCore.Http;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;

namespace Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;
public sealed class SendPrivateMessageByCurrentUserCommand : IRequest<Result<MessageDto>>
{
    public Guid ConversationId { get; set; }
    public string ReceiverId { get; set; } = null!;
    public string? Content { get; set; }

    public IEnumerable<IFormFile>? Attachments = [];
}
