using MediatR;
using Microsoft.AspNetCore.Http;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Emails.Commands.SendEmailWithAttachments;
public sealed class SendEmailWithAttachmentsCommand : IRequest<Result<bool>>
{
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
    public IEnumerable<IFormFile> Attachments { get; set; } = [];
}
