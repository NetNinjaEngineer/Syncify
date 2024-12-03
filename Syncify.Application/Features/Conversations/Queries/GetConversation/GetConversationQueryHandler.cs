using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Conversations.Queries.GetConversation;
public sealed class GetConversationQueryHandler(
    IConversationService conversationService) : IRequestHandler<GetConversationQuery, Result<ConversationDto>>
{
    public async Task<Result<ConversationDto>> Handle(GetConversationQuery request,
        CancellationToken cancellationToken)
        => await conversationService.GetConversationBetweenAsync(request);
}
