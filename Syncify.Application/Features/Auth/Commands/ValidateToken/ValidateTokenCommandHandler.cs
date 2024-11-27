using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.ValidateToken;
public sealed class ValidateTokenCommandHandler(
    IAuthService authService) : IRequestHandler<ValidateTokenCommand, Result<ValidateTokenResponseDto>>
{
    public async Task<Result<ValidateTokenResponseDto>> Handle(
        ValidateTokenCommand request,
        CancellationToken cancellationToken)
        => await authService.ValidateTokenAsync(request);
}
