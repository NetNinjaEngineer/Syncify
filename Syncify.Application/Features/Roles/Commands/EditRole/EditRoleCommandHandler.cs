using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Roles.Commands.EditRole;
public sealed class EditRoleCommandHandler(IRoleService roleService)
    : IRequestHandler<EditRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(
        EditRoleCommand request,
        CancellationToken cancellationToken)
        => await roleService.EditRole(request);
}
