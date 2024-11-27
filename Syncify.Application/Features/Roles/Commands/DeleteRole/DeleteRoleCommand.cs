using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Commands.DeleteRole;
public sealed class DeleteRoleCommand : IRequest<Result<string>>
{
    public string RoleName { get; set; } = string.Empty;

}
