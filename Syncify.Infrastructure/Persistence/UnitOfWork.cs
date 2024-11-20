using Syncify.Domain.Interfaces;

namespace Syncify.Infrastructure.Persistence;
public sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    public async ValueTask DisposeAsync() => await context.DisposeAsync();

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}
