using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Commands.EditMessage;
public sealed class EditMessageCommandHandler(
    IMessageService messageService) : IRequestHandler<EditMessageCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(
        EditMessageCommand request, CancellationToken cancellationToken)
        => await messageService.EditMessageAsync(request.MessageId, request.NewContent);
}
