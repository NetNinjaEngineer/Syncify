using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
public sealed class SendPrivateMessageCommandHandler(
    IMessageService messageService) : IRequestHandler<SendPrivateMessageCommand, Result<MessageDto>>
{
    public async Task<Result<MessageDto>> Handle(SendPrivateMessageCommand request, CancellationToken cancellationToken)
        => await messageService.SendPrivateMessageAsync(request);
}
