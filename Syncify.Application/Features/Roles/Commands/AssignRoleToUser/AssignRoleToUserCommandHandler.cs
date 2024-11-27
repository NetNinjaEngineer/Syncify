using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Roles.Commands.AssignRoleToUser;
public sealed class AssignRoleToUserCommandHandler
    (IRoleService roleService) : IRequestHandler<AssignRoleToUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AssignRoleToUserCommand request,
        CancellationToken cancellationToken)
        => await roleService.AddRoleToUser(request);
}
