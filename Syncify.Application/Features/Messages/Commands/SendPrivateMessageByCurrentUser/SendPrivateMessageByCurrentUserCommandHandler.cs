using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;
public sealed class SendPrivateMessageByCurrentUserCommandHandler(
    IMessageService messageService) : IRequestHandler<SendPrivateMessageByCurrentUserCommand, Result<MessageDto>>
{
    public async Task<Result<MessageDto>> Handle(
        SendPrivateMessageByCurrentUserCommand request, CancellationToken cancellationToken)
        => await messageService.SendPrivateMessageByCurrentUserAsync(request);
}
