using Syncify.Application.Bases;
using Syncify.Application.Features.Roles.Commands.AddClaimToRole;
using Syncify.Application.Features.Roles.Commands.AssignClaimToUser;
using Syncify.Application.Features.Roles.Commands.AssignRoleToUser;
using Syncify.Application.Features.Roles.Commands.CreateRole;
using Syncify.Application.Features.Roles.Commands.DeleteRole;
using Syncify.Application.Features.Roles.Commands.EditRole;

namespace Syncify.Application.Interfaces.Services;
public interface IRoleService
{
    Task<Result<string>> CreateRole(CreateRoleCommand request);
    Task<Result<string>> EditRole(EditRoleCommand request);
    Task<Result<string>> DeleteRole(DeleteRoleCommand request);
    Task<Result<string>> AddClaimToRole(AddClaimToRoleCommand request);
    Task<Result<string>> AddRoleToUser(AssignRoleToUserCommand request);
    Task<Result<string>> AddClaimToUser(AssignClaimToUserCommand request);
    Task<Result<IEnumerable<string>>> GetUserRoles(string userId);
    Task<Result<IEnumerable<string>>> GetUserClaims(string userId);
    Task<Result<IEnumerable<string>>> GetRoleClaims(string roleName);
    Task<Result<IEnumerable<string>>> GetAllRoles();
}
