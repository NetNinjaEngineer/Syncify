using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Conversations.Commands.StartConversation;
public sealed class StartConversationCommandHandler(IConversationService conversationService) :
    IRequestHandler<StartConversationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        StartConversationCommand request, CancellationToken cancellationToken)
        => await conversationService.StartConversationAsync(request);
}
