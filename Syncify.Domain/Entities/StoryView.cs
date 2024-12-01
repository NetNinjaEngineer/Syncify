using Syncify.Domain.Entities.Identity;

namespace Syncify.Domain.Entities;
public sealed class StoryView : BaseEntity
{
    public Guid? StoryId { get; set; }

    public Story? Story { get; set; }

    public string ViewerId { get; set; } = null!;

    public ApplicationUser Viewer { get; set; } = null!;
}
