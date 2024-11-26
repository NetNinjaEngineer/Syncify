using Syncify.Application.Bases;

namespace Syncify.Application.Interfaces.Services;
public interface IFollowingService
{
    Task<Result<bool>> UnfollowUserAsync(string followerId, string followedId);

    Task<Result<bool>> FollowUserAsync(string userFollowerId, string userToFollowId);
}
