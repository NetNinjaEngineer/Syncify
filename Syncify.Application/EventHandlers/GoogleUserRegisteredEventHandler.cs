using MediatR;
using Microsoft.Extensions.Logging;
using Syncify.Domain.Events;

namespace Syncify.Application.EventHandlers;
public sealed class GoogleUserRegisteredEventHandler(
    ILogger<GoogleUserRegisteredEventHandler> logger) : INotificationHandler<GoogleUserRegisteredEvent>
{
    public async Task Handle(GoogleUserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Welcome {notification.Name}, You can now access all features of your account.");
    }
}
