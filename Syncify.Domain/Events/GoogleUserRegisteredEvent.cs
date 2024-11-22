using MediatR;

namespace Syncify.Domain.Events;
public sealed class GoogleUserRegisteredEvent(string email, string name) : INotification
{
    public string Email { get; } = email;
    public string Name { get; } = name;
    public DateTime RegisteredAt { get; } = DateTime.Now;
}
