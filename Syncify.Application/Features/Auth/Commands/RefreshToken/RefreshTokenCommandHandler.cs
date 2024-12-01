using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.RefreshToken;
public sealed class RefreshTokenCommandHandler(IAuthService authService) : IRequestHandler<RefreshTokenCommand, Result<SignInResponseDto>>
{
    public async Task<Result<SignInResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        => await authService.RefreshTokenAsync(request);
}