using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Roles.Commands.AddClaimToRole;
public sealed class AddClaimToRoleCommandHandler
    (IRoleService roleService) : IRequestHandler<AddClaimToRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddClaimToRoleCommand request, CancellationToken cancellationToken)
        => await roleService.AddClaimToRole(request);
}
