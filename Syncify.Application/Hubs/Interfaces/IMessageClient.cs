using Syncify.Application.DTOs.Messages;

namespace Syncify.Application.Hubs.Interfaces;
public interface IMessageClient
{
    Task ReceivePrivateMessage(MessageDto message);
}
