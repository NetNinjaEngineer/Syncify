using Microsoft.AspNetCore.Http;

namespace Syncify.Application.Interfaces.Services.Models;

public sealed class EmailMessageWithAttachments : EmailMessage
{
    public IEnumerable<IFormFile> Attachments { get; set; } = [];
}