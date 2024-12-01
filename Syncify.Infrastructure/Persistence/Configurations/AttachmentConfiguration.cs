using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;
using Syncify.Domain.Enums;

namespace Syncify.Infrastructure.Persistence.Configurations;
internal sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.AttachmentType)
            .HasConversion(
                x => x.ToString(),
                x => Enum.Parse<AttachmentType>(x.ToString()));

        builder.HasOne(x => x.Message)
            .WithMany(x => x.Attachments)
            .HasForeignKey(x => x.MessageId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

    }
}
