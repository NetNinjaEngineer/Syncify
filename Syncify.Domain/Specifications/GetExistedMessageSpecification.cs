using Syncify.Domain.Entities;

namespace Syncify.Domain.Specifications;
public sealed class GetExistedMessageSpecification : BaseSpecification<Message>
{
    public GetExistedMessageSpecification(Guid messageId) : base(m => m.Id == messageId)
    {
        AddIncludes(m => m.Attachments);
        AddIncludes(m => m.Conversation);
        AddOrderByDescending(m => m.CreatedAt);
        DisableTracking();
    }

    public GetExistedMessageSpecification(Guid messageId, Guid conversationId) : base(m => m.Id == messageId && m.ConversationId == conversationId)
    {
        AddIncludes(m => m.Attachments);
    }
}
