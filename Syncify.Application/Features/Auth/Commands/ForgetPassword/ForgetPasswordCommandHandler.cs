using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.ForgetPassword;
public sealed class ForgetPasswordCommandHandler(
    IAuthService authService) : IRequestHandler<ForgetPasswordCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        ForgetPasswordCommand request,
        CancellationToken cancellationToken)
        => await authService.ForgotPasswordAsync(request);
}
