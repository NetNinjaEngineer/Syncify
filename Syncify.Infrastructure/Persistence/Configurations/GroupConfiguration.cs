using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;

namespace Syncify.Infrastructure.Persistence.Configurations;
public sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Id).ValueGeneratedNever();

        builder.Property(g => g.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne(g => g.CreatedByUser)
            .WithOne()
            .HasForeignKey<Group>(g => g.CreatedByUserId)
            .IsRequired();
    }
}
