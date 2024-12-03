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

        builder.HasDiscriminator<string>("ConversationType")
            .HasValue<PrivateConversation>("Private")
            .HasValue<GroupConversation>("Group");

        builder.Property<string>("ConversationType")
            .HasColumnType("VARCHAR");

        builder.ToTable("Conversations");

    }
}
