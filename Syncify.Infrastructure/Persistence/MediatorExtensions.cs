using MediatR;
using Syncify.Domain.Entities;

namespace Syncify.Infrastructure.Persistence;
public static class MediatorExtensions
{
    public static async Task DispatchDomainEventsAsync(this IMediator mediator,
        ApplicationDbContext context,
        CancellationToken cancellationToken)
    {
        var domainEntities = context.ChangeTracker
            .Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents != null && e.Entity.DomainEvents.Any());

        var domainEvents = domainEntities
            .SelectMany(d => d.Entity.DomainEvents!)
            .ToList();

        foreach (var notification in domainEvents)
            await mediator.Publish(notification, cancellationToken);
    }
}
