using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.ConfirmEmail;
public sealed class ConfirmEmailCommandHandler(
    IAuthService authService) : IRequestHandler<ConfirmEmailCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        => await authService.ConfirmEmailAsync(request);
}
