using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;

namespace Syncify.Application.Interfaces.Services;
public interface IMessageService
{
    Task<Result<MessageDto>> SendPrivateMessageAsync(SendPrivateMessageCommand command);
}
