using Syncify.Domain.Entities.Identity;

namespace Syncify.Domain.Entities;

public class PrivateConversation : Conversation
{
    public string SenderUserId { get; set; } = null!;
    public string ReceiverUserId { get; set; } = null!;
    public ApplicationUser SenderUser { get; set; } = null!;
    public ApplicationUser ReceiverUser { get; set; } = null!;
}
