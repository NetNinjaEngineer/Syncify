using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.SendConfirmEmailCode;
public sealed class SendConfirmEmailCodeCommandHandler(
    IAuthService authService) : IRequestHandler<SendConfirmEmailCodeCommand, Result<SendCodeConfirmEmailResponseDto>>
{
    public async Task<Result<SendCodeConfirmEmailResponseDto>> Handle(
        SendConfirmEmailCodeCommand request,
        CancellationToken cancellationToken)
        => await authService.SendConfirmEmailCodeAsync(request);
}
