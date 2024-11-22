using MediatR;

namespace Syncify.Domain.Entities;
public abstract class BaseEntity
{
    private List<INotification>? _domainEvents;

    public Guid Id { get; set; }

    public IReadOnlyCollection<INotification>? DomainEvents => _domainEvents?.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent)
    {
        _domainEvents ??= [];
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(INotification domainEvent) => _domainEvents?.Remove(domainEvent);

    public void ClearDomainEvent() => _domainEvents?.Clear();
}
