using Syncify.Domain.Enums;

namespace Syncify.Application.DTOs.Messages;
public sealed class AttachmentDto
{
    public string Url { get; set; } = string.Empty;
    public double Size { get; set; }
    public AttachmentType Type { get; set; }
}