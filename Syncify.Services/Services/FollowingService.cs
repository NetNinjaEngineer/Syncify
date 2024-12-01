using Microsoft.AspNetCore.Identity;
using Syncify.Application.Bases;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Interfaces;
using Syncify.Domain.Specifications;
using System.Net;

namespace Syncify.Services.Services;
public sealed class FollowingService(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork) : IFollowingService
{
    // must be called by user that have a role user
    public async Task<Result<bool>> UnfollowUserAsync(string followerId, string followedId)
    {
        if (string.Equals(followerId, followedId, StringComparison.CurrentCultureIgnoreCase))
            return Result<bool>.Failure(
                HttpStatusCode.Conflict, DomainErrors.Following.CanNotUnFollowYourself);

        var followerUser = await userManager.FindByIdAsync(followerId);

        var followedUser = await userManager.FindByIdAsync(followedId);

        if (followedUser == null || followerUser == null)
            return Result<bool>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UserNotExists);

        // check if its have a follow for this user

        var checkExistingUserFollowingSpec = new CheckExistingUserFollowingSpecification(followerId, followedId);

        var existingFollowing = await unitOfWork.Repository<UserFollower>()!.GetBySpecificationAsync(checkExistingUserFollowingSpec);

        if (existingFollowing is null)
            return Result<bool>.Failure(HttpStatusCode.BadRequest, DomainErrors.Following.NoFollowing);

        unitOfWork.Repository<UserFollower>()!.Delete(existingFollowing);

        await unitOfWork.SaveChangesAsync();

        // send real notification
        // enhance code

        return Result<bool>.Success(true, AppConstants.Following.UnfollowDone);

    }

    public async Task<Result<bool>> FollowUserAsync(string userFollowerId, string userToFollowId)
    {
        if (string.Equals(userFollowerId, userToFollowId, StringComparison.CurrentCultureIgnoreCase))
            return Result<bool>.Failure(HttpStatusCode.Conflict, DomainErrors.Following.CanNotFollowYourself);

        var followerUser = await userManager.FindByIdAsync(userFollowerId);

        var followedUser = await userManager.FindByIdAsync(userToFollowId);

        if (followedUser == null || followerUser == null)
            return Result<bool>.Failure(HttpStatusCode.NotFound, DomainErrors.Users.UserNotExists);

        var following = new UserFollower()
        {
            FollowerId = userFollowerId,
            FollowedId = userToFollowId,
            FollowedAt = DateTimeOffset.Now
        };

        unitOfWork.Repository<UserFollower>()!.Create(following);

        await unitOfWork.SaveChangesAsync();

        // send realtime notification for the userFollowed that another user has followed him

        return Result<bool>.Success(true, string.Format(
            AppConstants.Following.FollowingStarted,
            GetUserFullName(followerUser),
            GetUserFullName(followedUser),
            following.FollowedAt));
    }

    private static string GetUserFullName(ApplicationUser user) => string.Concat(user.FirstName, " ", user.LastName);
}
