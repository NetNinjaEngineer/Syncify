using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;

namespace Syncify.Application.Interfaces.Services;
public interface IMessageService
{
    Task<Result<MessageDto>> SendPrivateMessageAsync(SendPrivateMessageCommand command);
    Task<Result<MessageDto>> SendPrivateMessageByCurrentUserAsync(SendPrivateMessageByCurrentUserCommand command);
}
