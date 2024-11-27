using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Queries.GetAllRoles;
public sealed class GetAllRolesQuery : IRequest<Result<IEnumerable<string>>>
{
}
