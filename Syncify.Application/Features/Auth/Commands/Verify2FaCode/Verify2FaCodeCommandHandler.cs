using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.Verify2FaCode;
public sealed class Verify2FaCodeCommandHandler(IAuthService authService)
    : IRequestHandler<Verify2FaCodeCommand, Result<SignInResponseDto>>
{
    public async Task<Result<SignInResponseDto>> Handle(Verify2FaCodeCommand request,
        CancellationToken cancellationToken)
        => await authService.Verify2FaCodeAsync(request);
}