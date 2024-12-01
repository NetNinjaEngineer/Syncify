namespace Syncify.Application.Hubs.Interfaces;
public interface IFriendRequestClient
{
    Task ReceiveFriendRequest(string message);
}
