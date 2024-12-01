using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Emails.Commands.SendBulkEmails;
public sealed class SendBulkEmailsCommand : IRequest<Result<bool>>
{
    public List<string> ToRecepients { get; set; } = [];
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
}
