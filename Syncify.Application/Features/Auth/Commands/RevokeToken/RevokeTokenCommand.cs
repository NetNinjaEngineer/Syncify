using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Auth.Commands.RevokeToken;
public sealed class RevokeTokenCommand : IRequest<Result<bool>>
{
    public string? Token { get; set; }
}