using MediatR;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Syncify.Domain.Entities;
using Syncify.Domain.Entities.Identity;

namespace Syncify.Infrastructure.Persistence;
public sealed class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options,
    IMediator mediator) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Friendship> FriendshipRequests { get; set; }
    public DbSet<UserFollower> UserFollowers { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        await mediator.DispatchDomainEventsAsync(this, cancellationToken);
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
