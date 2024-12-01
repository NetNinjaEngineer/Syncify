using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;

namespace Syncify.Domain.Entities;
public sealed class Friendship : BaseEntity
{
    public string RequesterId { get; set; } = null!;

    public ApplicationUser Requester { get; set; } = null!;

    public required string ReceiverId { get; set; }

    public ApplicationUser Receiver { get; set; } = null!;

    public FriendshipStatus FriendshipStatus { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset? UpdatedAt { get; set; }

    public string? BlockedBy { get; set; }

    public ApplicationUser? BlockedByUser { get; set; }

    public string? BlockReason { get; set; }
}
