using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;

namespace Syncify.Application.Features.Conversations.Queries.GetConversationMessages;
public sealed class GetConversationMessagesQuery : IRequest<Result<ConversationDto>>
{
    public Guid ConversationId { get; set; }
}
