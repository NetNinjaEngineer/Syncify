using Syncify.Domain.Entities;

namespace Syncify.Domain.Specifications;

public sealed class CheckExistedConversationSpecification : BaseSpecification<Conversation>
{
    public CheckExistedConversationSpecification(Guid conversationId) : base(c => c.Id == conversationId)
    {
        AddIncludes(c => c.SenderUser);
        AddIncludes(c => c.ReceiverUser);
        AddIncludes(c => c.Messages);
    }
}