using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;

namespace Syncify.Application.Features.Auth.Commands.RefreshToken;
public sealed class RefreshTokenCommand : IRequest<Result<SignInResponseDto>>
{
    public string Token { get; set; } = string.Empty;
}