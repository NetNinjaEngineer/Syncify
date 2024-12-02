using Syncify.Application.Bases;
using Syncify.Application.Features.Conversations.Commands.StartConversation;

namespace Syncify.Application.Interfaces.Services;
public interface IConversationService
{
    Task<Result<string>> StartConversationAsync(StartConversationCommand command);
}
