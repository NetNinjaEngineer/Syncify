using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;

namespace Syncify.Application.Features.Conversations.Queries.GetConversation;
public sealed class GetConversationQuery : IRequest<Result<ConversationDto>>
{
    public required string SenderId { get; set; }
    public required string ReceiverId { get; set; }
}
