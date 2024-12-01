using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;

namespace Syncify.Domain.Entities;
public sealed class Story : BaseEntity
{
    public string MediaUrl { get; set; } = null!;

    public MediaType MediaType { get; set; }

    public string? Caption { get; set; }

    public List<string>? HashTags { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.Now;

    public DateTimeOffset? ExpiresAt { get; set; }

    public string UserId { get; set; } = null!;

    public ApplicationUser User { get; set; } = null!;

    public ICollection<StoryView> StoryViewers { get; set; } = [];
}
