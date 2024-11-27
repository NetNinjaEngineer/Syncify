using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Commands.CreateRole;
public sealed class CreateRoleCommand : IRequest<Result<string>>
{
    public string RoleName { get; set; } = string.Empty;
}
