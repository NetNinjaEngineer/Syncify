using MediatR;
using Microsoft.AspNetCore.Http;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Emails.Commands.SendlBulkEmailsWithAttachments;
public sealed class SendlBulkEmailsWithAttachmentsCommand : IRequest<Result<bool>>
{
    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public List<string> ToReceipients { get; set; } = [];

    public List<IFormFile> Attachments { get; set; } = [];
}
