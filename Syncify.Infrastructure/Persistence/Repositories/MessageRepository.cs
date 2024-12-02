using Microsoft.EntityFrameworkCore;
using Syncify.Domain.Entities;
using Syncify.Domain.Interfaces;

namespace Syncify.Infrastructure.Persistence.Repositories;
public sealed class MessageRepository(ApplicationDbContext context) : GenericRepository<Message>(context), IMessageRepository
{
    public async Task<Conversation?> GetConversationMessagesAsync(Guid conversationId)
    {
        var conversation = await context.Conversations
            .Include(c => c.Messages)
            .ThenInclude(m => m.Attachments)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        return conversation;
    }

    public async Task<Conversation?> GetPagedConversationMessagesAsync(
        Guid conversationId, int page, int size)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;
        if (size > 50) size = 50;

        var pagedConversationMessages = await context.Conversations
            .Include(
                c => c.Messages
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * size)
                .Take(size))
            .ThenInclude(m => m.Attachments)
            .Include(c => c.SenderUser)
            .Include(c => c.ReceiverUser)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        return pagedConversationMessages;
    }
}
