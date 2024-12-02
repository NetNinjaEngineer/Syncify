using Syncify.Application.DTOs.Messages;

namespace Syncify.Application.DTOs.Conversation;
public sealed class ConversationDto
{
    public string Sender { get; set; } = string.Empty;
    public string Receiver { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastMessageAt { get; set; }
    public IEnumerable<MessageDto> Messages { get; set; } = [];
}
