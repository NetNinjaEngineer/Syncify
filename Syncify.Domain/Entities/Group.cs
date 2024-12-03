using Syncify.Domain.Entities.Identity;

namespace Syncify.Domain.Entities;
public sealed class Group : BaseEntity
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;
    public string CreatedByUserId { get; set; } = null!;
    public ApplicationUser CreatedByUser { get; set; } = null!;
    public string? PictureName { get; set; }
    public ICollection<GroupConversation> GroupConversations { get; set; } = [];
    public ICollection<GroupMember> Members { get; set; } = [];
}
