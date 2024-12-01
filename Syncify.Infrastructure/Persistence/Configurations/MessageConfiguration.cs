using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;
using Syncify.Domain.Enums;

namespace Syncify.Infrastructure.Persistence.Configurations;
internal sealed class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.MessageStatus)
            .HasConversion(
                m => m.ToString(),
                m => Enum.Parse<MessageStatus>(m.ToString()));


        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
