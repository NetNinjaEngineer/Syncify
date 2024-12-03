using Syncify.Domain.Entities;

namespace Syncify.Domain.Interfaces;
public interface IConversationRepository : IGenericRepository<Conversation>
{
    Task<Conversation?> GetUserConversationsAsync(string userId);
    Task<Conversation?> GetConversationBetweenAsync(string senderId, string receiverId);
}
