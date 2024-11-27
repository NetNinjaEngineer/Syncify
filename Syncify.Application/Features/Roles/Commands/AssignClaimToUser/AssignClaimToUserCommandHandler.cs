using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Roles.Commands.AssignClaimToUser;
public sealed class AssignClaimToUserCommandHandler(IRoleService roleService)
    : IRequestHandler<AssignClaimToUserCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AssignClaimToUserCommand request, CancellationToken cancellationToken)
        => await roleService.AddClaimToUser(request);
}
