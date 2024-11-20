namespace Syncify.Domain.Interfaces;
public interface IUnitOfWork : IAsyncDisposable
{
    Task<int> SaveChangesAsync();
}
