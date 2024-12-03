using Microsoft.AspNetCore.Identity;
using Syncify.Domain.Enums;

namespace Syncify.Domain.Entities.Identity;
public sealed class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateOnly DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public string? CoverPhotoUrl { get; set; }

    public string? Bio { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset? UpdatedAt { get; set; }

    public string? Code { get; set; }

    public DateTimeOffset? CodeExpiration { get; set; }

    public ICollection<Friendship> SentFriendRequests { get; set; } = [];

    public ICollection<Friendship> ReceivedFriendRequests { get; set; } = [];

    public ICollection<UserFollower> Followers { get; set; } = [];

    public ICollection<UserFollower> FollowedUsers { get; set; } = [];

    public List<RefreshToken>? RefreshTokens { get; set; }

    public ICollection<Story> Stories { get; set; } = [];

    public ICollection<PrivateConversation> SentPrivateConversations { get; set; } = [];

    public ICollection<PrivateConversation> ReceivedPrivateConversations { get; set; } = [];
}