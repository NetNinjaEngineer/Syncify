using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.RevokeToken;
public sealed class RevokeTokenCommandHandler(IAuthService authService) : IRequestHandler<RevokeTokenCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
        => await authService.RevokeTokenAsync(request);
}
