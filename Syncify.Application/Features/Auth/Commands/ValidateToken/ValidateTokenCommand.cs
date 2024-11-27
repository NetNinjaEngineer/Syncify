using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;

namespace Syncify.Application.Features.Auth.Commands.ValidateToken;
public sealed class ValidateTokenCommand : IRequest<Result<ValidateTokenResponseDto>>
{
    public string JwtToken { get; set; } = null!;
}
