using MediatR;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.Logout;
public sealed class LogoutCommandHandler(IAuthService authService) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        => await authService.LogoutAsync();
}
