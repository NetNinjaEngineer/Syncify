using Syncify.Domain.Entities;

namespace Syncify.Domain.Interfaces;
public interface IMessageRepository : IGenericRepository<Message>
{
    Task<Conversation?> GetConversationMessagesAsync(Guid conversationId);
    Task<Conversation?> GetPagedConversationMessagesAsync(Guid conversationId, int page, int size);
}
