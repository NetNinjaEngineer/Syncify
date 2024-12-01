using Microsoft.AspNetCore.Identity;
using Syncify.Application.Bases;
using Syncify.Application.Features.Roles.Commands.AddClaimToRole;
using Syncify.Application.Features.Roles.Commands.AssignClaimToUser;
using Syncify.Application.Features.Roles.Commands.AssignRoleToUser;
using Syncify.Application.Features.Roles.Commands.CreateRole;
using Syncify.Application.Features.Roles.Commands.DeleteRole;
using Syncify.Application.Features.Roles.Commands.EditRole;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities.Identity;
using System.Net;
using System.Security.Claims;

namespace Syncify.Services.Services;
public sealed class RoleService(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ICurrentUser currentUser) : IRoleService
{
    public async Task<Result<string>> CreateRole(CreateRoleCommand request)
    {
        var roleExist = await roleManager.RoleExistsAsync(request.RoleName);
        if (roleExist)
        {
            return Result<string>.Failure(
                HttpStatusCode.Conflict, string.Format(DomainErrors.Roles.ErrorCreatingRole, request.RoleName));
        }

        var role = new IdentityRole(request.RoleName);
        var result = await roleManager.CreateAsync(role);

        return result.Succeeded
            ? Result<string>.Success(string.Format(AppConstants.Roles.RoleCreatedSuccessfully, request.RoleName))
            : Result<string>.Failure(
                HttpStatusCode.BadRequest,
                string.Format(DomainErrors.Roles.ErrorCreatingRole, request.RoleName));
    }

    public async Task<Result<string>> EditRole(EditRoleCommand request)
    {
        var role = await roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
        {
            return Result<string>.Failure(HttpStatusCode.NotFound, string.Format(DomainErrors.Roles.RoleNotFound, request.RoleName));
        }

        role.Name = request.NewRoleName;
        var result = await roleManager.UpdateAsync(role);

        return result.Succeeded
            ? Result<string>.Success(string.Format(AppConstants.Roles.RoleUpdatedSuccessfully, request.RoleName))
             : Result<string>.Failure(HttpStatusCode.BadRequest, string.Format(DomainErrors.Roles.ErrorUpdatingRole, request.RoleName));
    }

    public async Task<Result<string>> DeleteRole(DeleteRoleCommand request)
    {
        var role = await roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
        {
            return Result<string>.Failure(HttpStatusCode.NotFound,
                string.Format(DomainErrors.Roles.RoleNotFound, request.RoleName));
        }

        var result = await roleManager.DeleteAsync(role);

        return result.Succeeded
        ? Result<string>.Success(string.Format(AppConstants.Roles.RoleDeletedSuccessfully, role.Name))
            : Result<string>.Failure(HttpStatusCode.BadRequest, string.Format(DomainErrors.Roles.ErrorDeletingRole, role.Name));
    }

    public async Task<Result<string>> AddClaimToRole(AddClaimToRoleCommand request)
    {
        var role = await roleManager.FindByNameAsync(request.RoleName);
        if (role == null)
        {
            return Result<string>.Failure(
                HttpStatusCode.NotFound,
                string.Format(DomainErrors.Roles.RoleNotFound, request.RoleName));
        }

        var result = await roleManager.AddClaimAsync(role, new Claim(request.ClaimType, request.ClaimValue));

        return result.Succeeded
            ? Result<string>.Success(string.Format(AppConstants.Roles.ClaimAddedToRoleSuccessfully, request.ClaimType, request.ClaimValue, role.Name))
            : Result<string>.Failure(HttpStatusCode.BadRequest, string.Format(DomainErrors.Roles.ErrorAddingClaimToRole, role.Name));
    }
    public async Task<Result<IEnumerable<string>>> GetUserRoles(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<IEnumerable<string>>.Failure(HttpStatusCode.NotFound, string.Format(DomainErrors.Users.UserNotFound, userId));
        }

        var roles = await userManager.GetRolesAsync(user);
        return Result<IEnumerable<string>>.Success(roles);
    }

    public async Task<Result<IEnumerable<string>>> GetUserClaims(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<IEnumerable<string>>.Failure(HttpStatusCode.NotFound, string.Format(DomainErrors.Users.UserNotFound, currentUser.Id));
        }

        var claims = await userManager.GetClaimsAsync(user);
        return Result<IEnumerable<string>>.Success(claims.Select(a => a.Value).ToList());
    }

    public async Task<Result<IEnumerable<string>>> GetRoleClaims(string roleName)
    {
        var role = await roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            return Result<IEnumerable<string>>.Failure(HttpStatusCode.NotFound, string.Format(DomainErrors.Roles.RoleNotFound, roleName));
        }

        var claims = await roleManager.GetClaimsAsync(role);
        return Result<IEnumerable<string>>.Success(claims.Select(c => $"{c.Type}:{c.Value}"));
    }

    public async Task<Result<IEnumerable<string>>> GetAllRoles()
    {
        var roles = await Task.FromResult(roleManager.Roles.Select(r => r.Name));
        return Result<IEnumerable<string>>.Success(roles);
    }

    public async Task<Result<string>> AddRoleToUser(AssignRoleToUserCommand request)
    {
        var user = await userManager.FindByEmailAsync(request.UserId);
        if (user == null)
        {
            return Result<string>.Failure(HttpStatusCode.NotFound, string.Format(DomainErrors.Users.UserNotFound, request.UserId));
        }

        var result = await userManager.AddToRoleAsync(user, request.RoleName);

        return result.Succeeded
            ? Result<string>.Success(string.Format(AppConstants.Roles.RoleAssignedSuccessfully, request.RoleName, user.UserName))
            : Result<string>.Failure(HttpStatusCode.BadRequest, string.Format(DomainErrors.Roles.ErrorAssigningRole, request.RoleName, user.UserName));
    }

    public async Task<Result<string>> AddClaimToUser(AssignClaimToUserCommand request)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return Result<string>.Failure(
                HttpStatusCode.NotFound, string.Format(DomainErrors.Users.UserNotFound, request.UserId));
        }

        var result = await userManager.AddClaimAsync(user, new Claim(request.ClaimType, request.ClaimValue));

        return result.Succeeded
            ? Result<string>.Success(string.Format(AppConstants.Roles.ClaimAddedSuccessfully, request.ClaimType, request.ClaimValue, user.UserName))
            : Result<string>.Failure(HttpStatusCode.BadRequest, string.Format(DomainErrors.Roles.ErrorAddingClaim, user.UserName));
    }
}
