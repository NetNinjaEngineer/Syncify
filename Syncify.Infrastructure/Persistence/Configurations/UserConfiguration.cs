using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;

namespace Syncify.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.ProfilePictureUrl).IsRequired(false);

        builder.Property(x => x.CoverPhotoUrl).IsRequired(false);

        builder.Property(x => x.Bio)
            .HasColumnType("varchar")
            .HasMaxLength(255)
            .IsRequired(false);

        builder.Property(x => x.Gender)
            .HasConversion(
                g => g.ToString(),
                g => (Gender)Enum.Parse(typeof(Gender), g));


    }
}
