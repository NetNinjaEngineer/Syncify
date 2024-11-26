using Syncify.Domain.Entities;

namespace Syncify.Domain.Interfaces;
public interface IUnitOfWork : IAsyncDisposable
{
    IFriendshipRepository FriendshipRepository { get; }
    Task<int> SaveChangesAsync();
    IGenericRepository<TEntity>? Repository<TEntity>() where TEntity : BaseEntity;
}
