using MediatR;
using Syncify.Application.Bases;
using Syncify.Domain.Enums;

namespace Syncify.Application.Features.Auth.Commands.Enable2Fa;
public sealed class Enable2FaCommand : IRequest<Result<string>>
{
    public TokenProvider TokenProvider { get; set; }
    public string Email { get; set; } = null!;
}