using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;

namespace Syncify.Infrastructure.Persistence.Configurations;
internal sealed class UserFollowersConfiguration : IEntityTypeConfiguration<UserFollower>
{
    public void Configure(EntityTypeBuilder<UserFollower> builder)
    {
        builder.HasKey(uf => uf.Id);
        builder.Property(uf => uf.Id).ValueGeneratedNever();

        // user can have many followed users
        builder.HasOne(uf => uf.FollowerUser)
            .WithMany(u => u.FollowedUsers)
            .HasForeignKey(u => u.FollowerId);

        // follower can have many followed users
        builder.HasOne(uf => uf.FollowedUser)
            .WithMany(u => u.Followers)
            .HasForeignKey(uf => uf.FollowedId);

        builder.HasIndex(uf => new { uf.FollowedId, uf.FollowerId })
            .HasDatabaseName("IX_UserFollowers_Followed_Follower");

        builder.ToTable("UserFollowers",
            opt => opt.HasCheckConstraint("CHK_NoSelfFollow", "FollowedId <> FollowerId"));
    }
}
