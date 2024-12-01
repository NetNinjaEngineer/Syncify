using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Commands.AddClaimToRole;
public sealed class AddClaimToRoleCommand : IRequest<Result<string>>
{
    public string RoleName { get; set; } = string.Empty;
    public string ClaimType { get; set; } = string.Empty;
    public string ClaimValue { get; set; } = string.Empty;
}
