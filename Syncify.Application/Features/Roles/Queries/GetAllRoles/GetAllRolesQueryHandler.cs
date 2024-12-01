using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Roles.Queries.GetAllRoles;
public sealed class GetAllRolesQueryHandler(
    IRoleService roleService) : IRequestHandler<GetAllRolesQuery, Result<IEnumerable<string>>>
{
    public async Task<Result<IEnumerable<string>>> Handle(
        GetAllRolesQuery request,
        CancellationToken cancellationToken)
        => await roleService.GetAllRoles();
}
