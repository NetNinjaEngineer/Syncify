using Syncify.Domain.Entities.Identity;

namespace Syncify.Domain.Entities;
public sealed class UserFollower : BaseEntity
{
    public string? FollowedId { get; set; }

    public string? FollowerId { get; set; }

    public DateTimeOffset FollowedAt { get; set; }

    public ApplicationUser? FollowedUser { get; set; }

    public ApplicationUser? FollowerUser { get; set; }
}
