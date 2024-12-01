using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Queries.GetUserRoles;
public sealed class GetUserRolesQuery : IRequest<Result<IEnumerable<string>>>
{
    public string UserId { get; set; } = string.Empty;
}
