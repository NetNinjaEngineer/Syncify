using Syncify.Domain.Entities;

namespace Syncify.Domain.Specifications;
public sealed class CheckExistingUserFollowingSpecification(string followerId, string followedId)
    : BaseSpecification<UserFollower>(uf =>
        uf.FollowedId == followedId && uf.FollowerId == followerId);
