using Syncify.Domain.Enums;

namespace Syncify.Application.DTOs.Messages;
public sealed record MessageDto(
    string SenderName,
    string ReceiverName,
    MessageStatus MessageStatus,
    string? Content,
    List<AttachmentDto>? Attachments);