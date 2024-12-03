using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;

namespace Syncify.Domain.Entities;

public sealed class GroupMember : BaseEntity
{
    public Guid? GroupId { get; set; }
    public Group? Group { get; set; } = null!;
    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.Now;
    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public GroupMemberRole Role { get; set; }
}