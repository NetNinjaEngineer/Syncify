using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using Syncify.Application.Bases;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Application.Interfaces.Services.Models;
using System.Net;

namespace Syncify.Services.Services;
public sealed class MailService(IOptions<SmtpSettings> smtpSettingsOptions) : IMailService
{
    private readonly SmtpSettings _smtpSettings = smtpSettingsOptions.Value;

    public async Task<Result<bool>> SendEmailAsync(EmailMessage emailMessage)
    {
        var messageResult = CreateMimeMessage(emailMessage.To, emailMessage.Subject, emailMessage.Message);

        var isSent = await SendMailMessageAsync(messageResult.Value);

        return IsEmailSent(emailMessage, isSent.Value);
    }

    public async Task<Result<bool>> SendEmailWithAttachmentsAsync(EmailMessageWithAttachments emailMessage)
    {
        var messageResult = await CreateMimeMessage(
            emailMessage.To,
            emailMessage.Subject,
            emailMessage.Message,
            emailMessage.Attachments.ToList());

        var isSent = await SendMailMessageAsync(messageResult.Value);

        return IsEmailSent(emailMessage, isSent.Value);

    }

    private static Result<bool> IsEmailSent(EmailMessage emailMessage, bool isSent)
    {
        return isSent ?
            Result<bool>.Success(true, $"Email is sent to '{emailMessage.To}' successfully.")
            : Result<bool>.Failure(HttpStatusCode.BadRequest, "Email not sent.");
    }


    private Result<MimeMessage> CreateMimeMessage(string toEmail, string subject, string message)
    {
        var mimeMessage = new MimeMessage();
        var bodyBuilder = new BodyBuilder();

        mimeMessage.From.Add(new MailboxAddress(_smtpSettings.Gmail.SenderName, _smtpSettings.Gmail.SenderEmail));
        mimeMessage.To.Add(new MailboxAddress(toEmail, toEmail));
        mimeMessage.Subject = subject;
        bodyBuilder.TextBody = message;
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        return Result<MimeMessage>.Success(mimeMessage);
    }

    private async Task<Result<MimeMessage>> CreateMimeMessage(string toEmail,
        string subject, string message, List<IFormFile> attachments)
    {
        var mimeMessage = new MimeMessage();
        var bodyBuilder = new BodyBuilder();

        mimeMessage.From.Add(new MailboxAddress(_smtpSettings.Gmail.SenderName, _smtpSettings.Gmail.SenderEmail));
        mimeMessage.To.Add(new MailboxAddress(toEmail, toEmail));
        mimeMessage.Subject = subject;
        bodyBuilder.TextBody = message;

        if (attachments?.Count > 0)
        {
            foreach (var file in attachments)
            {
                var fileName = Path.GetFileName(file.FileName);
                await bodyBuilder.Attachments.AddAsync(fileName, file.OpenReadStream());
            }
        }

        mimeMessage.Body = bodyBuilder.ToMessageBody();

        return Result<MimeMessage>.Success(mimeMessage);
    }


    private async Task<Result<bool>> SendMailMessageAsync(MimeMessage message)
    {
        using var emailClient = new SmtpClient();

        await emailClient.ConnectAsync(_smtpSettings.Gmail.Host,
            _smtpSettings.Gmail.Port, SecureSocketOptions.StartTls, CancellationToken.None);

        await emailClient.AuthenticateAsync(_smtpSettings.Gmail.SenderEmail,
            _smtpSettings.Gmail.Password, CancellationToken.None);

        await emailClient.SendAsync(message, CancellationToken.None);

        await emailClient.DisconnectAsync(true);

        return Result<bool>.Success(true);
    }
}
