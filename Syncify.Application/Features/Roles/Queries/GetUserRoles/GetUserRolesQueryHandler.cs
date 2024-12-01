using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Roles.Queries.GetUserRoles;
public sealed class GetUserRolesQueryHandler(
    IRoleService roleService) : IRequestHandler<GetUserRolesQuery, Result<IEnumerable<string>>>
{
    public async Task<Result<IEnumerable<string>>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
        => await roleService.GetUserRoles(request.UserId);
}
