using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;
using Syncify.Domain.Enums;

namespace Syncify.Infrastructure.Persistence.Configurations;
public sealed class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
{
    public void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.HasOne(x => x.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(x => x.GroupId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Property(x => x.Role)
            .HasConversion(
                gm => gm.ToString(),
                gm => Enum.Parse<GroupMemberRole>(gm.ToString()));

        builder.HasOne(x => x.User)
            .WithOne()
            .HasForeignKey<GroupMember>(gm => gm.UserId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
