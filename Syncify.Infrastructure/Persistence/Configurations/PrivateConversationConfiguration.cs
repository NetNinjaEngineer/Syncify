using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Syncify.Domain.Entities;

namespace Syncify.Infrastructure.Persistence.Configurations;
public sealed class PrivateConversationConfiguration : IEntityTypeConfiguration<PrivateConversation>
{
    public void Configure(EntityTypeBuilder<PrivateConversation> builder)
    {
        builder.HasOne(c => c.SenderUser)
         .WithMany(u => u.SentPrivateConversations)
         .HasForeignKey(c => c.SenderUserId)
         .OnDelete(DeleteBehavior.Restrict)
         .IsRequired();

        builder.HasOne(c => c.ReceiverUser)
            .WithMany(u => u.ReceivedPrivateConversations)
            .HasForeignKey(c => c.ReceiverUserId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
    }
}
