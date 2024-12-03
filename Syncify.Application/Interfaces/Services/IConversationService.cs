using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.Features.Conversations.Commands.DeleteConversation;
using Syncify.Application.Features.Conversations.Commands.StartConversation;
using Syncify.Application.Features.Conversations.Queries.GetConversation;

namespace Syncify.Application.Interfaces.Services;
public interface IConversationService
{
    Task<Result<string>> StartConversationAsync(StartConversationCommand command);
    Task<Result<ConversationDto>> GetConversationBetweenAsync(GetConversationQuery getConversationQuery);
    Task<Result<bool>> DeleteConversationAsync(DeleteConversationCommand deleteConversationCommand);
}
