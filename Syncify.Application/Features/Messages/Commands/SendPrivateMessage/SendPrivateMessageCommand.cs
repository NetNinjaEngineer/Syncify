using MediatR;
using Microsoft.AspNetCore.Http;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;

namespace Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
public sealed class SendPrivateMessageCommand : IRequest<Result<MessageDto>>
{
    public Guid ConversationId { get; set; }
    public string SenderId { get; set; } = null!;
    public string ReceiverId { get; set; } = null!;
    public string? Content { get; set; }

    public IEnumerable<IFormFile>? Attachments = [];
}
