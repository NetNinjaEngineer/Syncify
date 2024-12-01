using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.ConfirmForgotPasswordCode;
public class ConfirmForgotPasswordCodeCommandHandler(IAuthService authService) :
    IRequestHandler<ConfirmForgotPasswordCodeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        ConfirmForgotPasswordCodeCommand request,
        CancellationToken cancellationToken)
        => await authService.ConfirmForgotPasswordCodeAsync(request);
}