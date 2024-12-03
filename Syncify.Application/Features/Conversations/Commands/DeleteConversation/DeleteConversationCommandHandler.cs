using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Conversations.Commands.DeleteConversation;
public sealed class DeleteConversationCommandHandler(
    IConversationService conversationService) : IRequestHandler<DeleteConversationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteConversationCommand request,
        CancellationToken cancellationToken)
        => await conversationService.DeleteConversationAsync(request);
}
