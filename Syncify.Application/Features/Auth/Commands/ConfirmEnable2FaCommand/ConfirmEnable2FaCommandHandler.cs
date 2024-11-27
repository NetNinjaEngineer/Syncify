using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.ConfirmEnable2FaCommand;
public sealed class ConfirmEnable2FaCommandHandler(IAuthService authService)
    : IRequestHandler<ConfirmEnable2FaCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmEnable2FaCommand request, CancellationToken cancellationToken)
        => await authService.ConfirmEnable2FaAsync(request);
}