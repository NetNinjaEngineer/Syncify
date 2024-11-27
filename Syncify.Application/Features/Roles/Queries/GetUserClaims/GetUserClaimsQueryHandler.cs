using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Roles.Queries.GetUserClaims;

public sealed class GetUserClaimsQueryHandler(
    IRoleService roleService) : IRequestHandler<GetUserClaimsQuery, Result<IEnumerable<string>>>
{
    public async Task<Result<IEnumerable<string>>> Handle(
        GetUserClaimsQuery request,
        CancellationToken cancellationToken)
        => await roleService.GetUserClaims(request.UserId);
}