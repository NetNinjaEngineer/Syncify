using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Commands.DeleteMessageInConversation;
public sealed class DeleteMessageInConversationCommandHandler(
    IMessageService messageService) : IRequestHandler<DeleteMessageInConversationCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(DeleteMessageInConversationCommand request,
        CancellationToken cancellationToken)
        => await messageService.DeleteMessageInConversationAsync(request);
}
