using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Commands.MarkMessageAsRead;
public sealed class MarkMessageAsReadCommandHandler(IMessageService messageService) : IRequestHandler<MarkMessageAsReadCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(MarkMessageAsReadCommand request,
        CancellationToken cancellationToken) => await messageService.MarkMessageAsReadAsync(request);
}
