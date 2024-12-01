using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services.Models;

namespace Syncify.Application.Interfaces.Services;
public interface IMailService
{
    Task<Result<bool>> SendEmailAsync(EmailMessage emailMessage);
    Task<Result<bool>> SendEmailWithAttachmentsAsync(EmailMessageWithAttachments emailMessage);
    Task<Result<bool>> SendBulkEmailsAsync(EmailBulk emailMessage);
    Task<Result<bool>> SendBulkEmailsWithAttachmentsAsync(EmailBulkWithAttachments emailMessage);
}
