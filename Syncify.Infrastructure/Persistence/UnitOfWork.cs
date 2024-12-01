using Microsoft.AspNetCore.Identity;
using Syncify.Domain.Entities;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Interfaces;
using Syncify.Infrastructure.Persistence.Repositories;
using System.Collections;

namespace Syncify.Infrastructure.Persistence;
public sealed class UnitOfWork(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : IUnitOfWork
{
    private readonly Hashtable _repositories = [];
    public IFriendshipRepository FriendshipRepository { get; } = new FriendshipRepository(context, userManager);

    public async Task<int> SaveChangesAsync() => await context.SaveChangesAsync();

    public IGenericRepository<TEntity>? Repository<TEntity>() where TEntity : BaseEntity
    {
        var type = typeof(TEntity).Name;
        if (_repositories.ContainsKey(type)) return _repositories[type] as IGenericRepository<TEntity>;
        var repository = new GenericRepository<TEntity>(context);
        _repositories.Add(type, repository);

        return _repositories[type] as IGenericRepository<TEntity>;
    }

    public async ValueTask DisposeAsync() => await context.DisposeAsync();

}
