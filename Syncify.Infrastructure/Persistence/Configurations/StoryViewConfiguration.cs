using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;

namespace Syncify.Infrastructure.Persistence.Configurations;
public sealed class StoryViewConfiguration : IEntityTypeConfiguration<StoryView>
{
    public void Configure(EntityTypeBuilder<StoryView> builder)
    {
        builder.HasKey(sv => sv.Id);

        builder.HasOne(sv => sv.Viewer)
            .WithMany()
            .HasForeignKey(sv => sv.ViewerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(sv => new { sv.StoryId, sv.ViewerId })
            .IsUnique();

        builder.ToTable("StoryViews");
    }
}
