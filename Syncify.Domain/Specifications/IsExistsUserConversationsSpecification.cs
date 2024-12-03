using Syncify.Domain.Entities;

namespace Syncify.Domain.Specifications;
public sealed class IsExistsUserConversationsSpecification : BaseSpecification<PrivateConversation>
{
    public IsExistsUserConversationsSpecification(string userId) : base(c =>
        string.IsNullOrEmpty(userId) || c.SenderUserId == userId || c.ReceiverUserId == userId)
    {
    }
}
