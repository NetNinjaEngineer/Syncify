namespace Syncify.Application.Hubs.Interfaces;
public interface INotificationClient
{
    Task ReceiveNotification(string message);
}
