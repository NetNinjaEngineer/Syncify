using Microsoft.AspNetCore.Identity;
using Syncify.Domain.Enums;

namespace Syncify.Domain.Entities.Identity;
public sealed class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public DateTimeOffset DateOfBirth { get; set; }

    public Gender Gender { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public string? CoverPhotoUrl { get; set; }

    public string? Bio { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public IEnumerable<Friendship> SentFriendRequests { get; set; } = [];

    public IEnumerable<Friendship> ReceivedFriendRequests { get; set; } = [];
}
