using Microsoft.EntityFrameworkCore;
using Syncify.Domain.Entities;
using Syncify.Domain.Interfaces;

namespace Syncify.Infrastructure.Persistence.Repositories;
public sealed class ConversationRepository(ApplicationDbContext context) :
    GenericRepository<Conversation>(context), IConversationRepository
{
    public async Task<Conversation?> GetConversationBetweenAsync(string senderId, string receiverId)
    {
        return await context.Conversations
            .AsNoTracking()
            .Include(conversation => conversation.SenderUser)
            .Include(conversation => conversation.ReceiverUser)
            .Include(conversation => conversation.Messages)
            .ThenInclude(message => message.Attachments)
            .FirstOrDefaultAsync(conversation =>
                conversation.SenderUserId == senderId && conversation.ReceiverUserId == receiverId);
    }

    public async Task<Conversation?> GetUserConversationsAsync(string userId)
    {
        var userConversation = await context.Conversations
            .Include(conversation => conversation.SenderUser)
            .Include(conversation => conversation.ReceiverUser)
            .Include(conversation => conversation
                .Messages.OrderByDescending(message => message.CreatedAt))
            .ThenInclude(message => message.Attachments)
            .FirstOrDefaultAsync(conversation =>
                conversation.SenderUserId == userId ||
                conversation.ReceiverUserId == userId);

        return userConversation;
    }
}
