using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Commands.AssignClaimToUser;
public sealed class AssignClaimToUserCommand : IRequest<Result<string>>
{
    public string UserId { get; set; } = string.Empty;
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
}
