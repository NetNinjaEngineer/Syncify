using Syncify.Domain.Enums;

namespace Syncify.Application.DTOs.Messages;
public sealed record AttachmentDto(string Url, double Size, AttachmentType Type);