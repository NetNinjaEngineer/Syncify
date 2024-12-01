using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandHandler(
    IAuthService authService) : IRequestHandler<RegisterCommand, Result<RegisterResponseDto>>
{
    public async Task<Result<RegisterResponseDto>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
        => await authService.RegisterAsync(request);
}