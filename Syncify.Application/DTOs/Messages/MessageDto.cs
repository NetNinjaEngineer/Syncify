using Syncify.Domain.Enums;

namespace Syncify.Application.DTOs.Messages;

public sealed class MessageDto(
    string senderName,
    string receiverName,
    MessageStatus messageStatus,
    string? content,
    List<AttachmentDto>? attachments)
{
    public string SenderName { get; private set; } = senderName;
    public string ReceiverName { get; private set; } = receiverName;
    public MessageStatus MessageStatus { get; private set; } = messageStatus;
    public string? Content { get; private set; } = content;
    public List<AttachmentDto>? Attachments { get; private set; } = attachments;
}