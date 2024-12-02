using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Conversations.Queries.GetConversationMessages;
public sealed class GetConversationMessagesQueryHandler(IMessageService messageService)
    : IRequestHandler<GetConversationMessagesQuery, Result<ConversationDto>>
{
    public async Task<Result<ConversationDto>> Handle(GetConversationMessagesQuery request, CancellationToken cancellationToken)
        => await messageService.GetConversationMessagesAsync(request.ConversationId);
}
