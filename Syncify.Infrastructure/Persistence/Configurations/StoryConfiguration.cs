using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;
using Syncify.Domain.Enums;
using System.Text.Json;

namespace Syncify.Infrastructure.Persistence.Configurations;
internal sealed class StoryConfiguration : IEntityTypeConfiguration<Story>
{
    public void Configure(EntityTypeBuilder<Story> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.MediaUrl)
            .IsRequired();

        builder.Property(s => s.MediaType)
            .HasConversion(
                m => m.ToString(),
                m => (MediaType)Enum.Parse(typeof(MediaType), m));

        builder.Property(s => s.Caption)
            .HasMaxLength(500);

        builder.Property(s => s.HashTags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
                v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>()
            );

        builder.HasOne(s => s.User)
            .WithMany(u => u.Stories)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.StoryViewers)
            .WithOne(sv => sv.Story)
            .HasForeignKey(sv => sv.StoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(s => s.UserId);

        builder.ToTable("Stories");
    }
}
