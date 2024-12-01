using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;

namespace Syncify.Infrastructure.Persistence.Configurations;
internal sealed class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.HasOne(c => c.SenderUser)
            .WithMany(u => u.SentConversations)
            .HasForeignKey(c => c.SenderUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.HasOne(c => c.ReceiverUser)
            .WithMany(u => u.ReceivedConversations)
            .HasForeignKey(c => c.ReceiverUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();

        builder.ToTable("Conversations");

    }
}
