using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;

namespace Syncify.Application.Features.Conversations.Queries.GetUserConversation;
public sealed class GetUserConversationQuery : IRequest<Result<ConversationDto>>
{
    public string UserId { get; set; } = null!;
}
