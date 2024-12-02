using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;

namespace Syncify.Application.Features.Conversations.Queries.GetPagedConversationMessages;
public sealed class GetPagedConversationMessagesQuery : IRequest<Result<ConversationDto>>
{
    public Guid ConversationId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
