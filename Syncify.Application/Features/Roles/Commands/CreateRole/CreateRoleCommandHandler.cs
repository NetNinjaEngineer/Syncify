using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Roles.Commands.CreateRole;
public sealed class CreateRoleCommandHandler
    (IRoleService roleService) : IRequestHandler<CreateRoleCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        => await roleService.CreateRole(request);
}
