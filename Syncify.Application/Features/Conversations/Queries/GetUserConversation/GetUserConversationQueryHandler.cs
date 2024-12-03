using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Conversations.Queries.GetUserConversation;
public sealed class GetUserConversationQueryHandler(
    IMessageService messageService) : IRequestHandler<GetUserConversationQuery, Result<ConversationDto>>
{
    public async Task<Result<ConversationDto>> Handle(GetUserConversationQuery request,
        CancellationToken cancellationToken)
        => await messageService.GetUserConversationsAsync(request);
}
