using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;
using Syncify.Application.Interfaces.Services.Models;

namespace Syncify.Application.Features.Emails.Commands.SendEmailWithAttachments;
public sealed class SendEmailWithAttachmentsCommandHandler(IMailService mailService) :
    IRequestHandler<SendEmailWithAttachmentsCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(SendEmailWithAttachmentsCommand request,
        CancellationToken cancellationToken) =>
        await mailService.SendEmailWithAttachmentsAsync(
            new EmailMessageWithAttachments()
            {
                To = request.To,
                Message = request.Message,
                Subject = request.Subject,
                Attachments = request.Attachments
            });
}
