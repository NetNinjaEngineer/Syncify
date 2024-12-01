using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;
using Syncify.Domain.Enums;

namespace Syncify.Infrastructure.Persistence.Configurations;
internal sealed class FriendshipConfiguration : IEntityTypeConfiguration<Friendship>
{
    public void Configure(EntityTypeBuilder<Friendship> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Id).ValueGeneratedNever();

        builder.HasOne(f => f.Requester)
            .WithMany(u => u.SentFriendRequests)
            .HasForeignKey(f => f.RequesterId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(f => f.Receiver)
            .WithMany(u => u.ReceivedFriendRequests)
            .HasForeignKey(f => f.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(f => f.BlockedByUser)
            .WithMany()
            .HasForeignKey(f => f.BlockedBy)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);

        builder.Property(f => f.BlockReason)
            .HasMaxLength(500).IsRequired(false);

        builder.Property(f => f.FriendshipStatus)
            .HasConversion(
                fs => fs.ToString(),
                fs => (FriendshipStatus)Enum.Parse(typeof(FriendshipStatus), fs));

        builder.ToTable("Friendships");
    }
}
