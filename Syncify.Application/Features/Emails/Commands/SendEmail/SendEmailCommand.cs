using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Emails.Commands.SendEmail;
public sealed class SendEmailCommand : IRequest<Result<bool>>
{
    public string To { get; set; } = null!;
    public string Subject { get; set; } = null!;
    public string Message { get; set; } = null!;
}
