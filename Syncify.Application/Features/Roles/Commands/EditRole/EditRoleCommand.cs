using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Commands.EditRole;
public sealed class EditRoleCommand : IRequest<Result<string>>
{
    public string RoleName { get; set; } = string.Empty;
    public string NewRoleName { get; set; } = string.Empty;
}
