using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Commands.AssignRoleToUser;
public sealed class AssignRoleToUserCommand : IRequest<Result<string>>
{
    public string UserId { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
}
