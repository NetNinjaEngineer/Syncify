using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Conversations.Queries.GetPagedConversationMessages;
public sealed class GetPagedConversationMessagesQueryHandler(
    IMessageService messageService) : IRequestHandler<GetPagedConversationMessagesQuery, Result<ConversationDto>>
{
    public async Task<Result<ConversationDto>> Handle(
        GetPagedConversationMessagesQuery request,
        CancellationToken cancellationToken) => await messageService.GetConversationMessagesAsync(request);
}
