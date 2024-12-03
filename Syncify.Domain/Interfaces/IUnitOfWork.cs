using Syncify.Domain.Entities;

namespace Syncify.Domain.Interfaces;
public interface IUnitOfWork : IAsyncDisposable
{
    IFriendshipRepository FriendshipRepository { get; }
    IMessageRepository MessageRepository { get; }
    IConversationRepository ConversationRepository { get; }
    IGenericRepository<TEntity>? Repository<TEntity>() where TEntity : BaseEntity;
    Task<int> SaveChangesAsync();
}
