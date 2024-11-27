using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Queries.GetRoleClaims;
public sealed class GetRoleClaimsQuery : IRequest<Result<IEnumerable<string>>>
{
    public string RoleName { get; set; } = string.Empty;
}
