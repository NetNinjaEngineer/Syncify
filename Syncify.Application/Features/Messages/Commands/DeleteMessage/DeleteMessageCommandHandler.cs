using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Commands.DeleteMessage;
public sealed class DeleteMessageCommandHandler(
    IMessageService messageService) : IRequestHandler<DeleteMessageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        DeleteMessageCommand request,
        CancellationToken cancellationToken)
        => await messageService.DeleteMessageAsync(request.MessageId);
}
