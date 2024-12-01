using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Auth.Commands.ConfirmEmail;
public sealed class ConfirmEmailCommand : IRequest<Result<string>>
{
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
