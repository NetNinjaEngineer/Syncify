using Microsoft.EntityFrameworkCore;
using Syncify.Domain.Entities;
using Syncify.Domain.Enums;
using Syncify.Domain.Interfaces;

namespace Syncify.Infrastructure.Persistence.Repositories;
public sealed class MessageRepository(ApplicationDbContext context) :
    GenericRepository<Message>(context), IMessageRepository
{
    public async Task<Conversation?> GetConversationMessagesAsync(Guid conversationId)
    {
        var conversation = await context.Conversations
            .Include(c => c.Messages)
            .ThenInclude(m => m.Attachments)
            .FirstOrDefaultAsync(c => c.Id == conversationId);

        return conversation;
    }

    public async Task<IEnumerable<Message>> GetMessagesByDateRangeAsync(
        DateOnly startDate,
        DateOnly endDate,
        Guid? conversationId = null)
    {
        var todayDate = DateOnly.FromDateTime(DateTime.Today);

        if (startDate > todayDate)
            startDate = todayDate;

        if (endDate > todayDate)
            endDate = todayDate;

        if (endDate < todayDate)
            endDate = todayDate;

        var messages = await context.Messages
            .AsNoTracking()
            .Include(message => message.Attachments)
            .Include(message => message.Conversation)
            .ThenInclude(conversation => conversation.SenderUser)
            .Include(message => message.Conversation)
            .ThenInclude(conversation => conversation.ReceiverUser)
            .Where(message =>
                DateOnly.FromDateTime(message.CreatedAt.Date) >= startDate &&
                DateOnly.FromDateTime(message.CreatedAt.Date) <= endDate &&
                (!conversationId.HasValue || message.ConversationId == conversationId))
            .ToListAsync();

        return messages;
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

    public async Task<IEnumerable<Message>> GetUnreadMessagesAsync(string userId)
    {
        var unreadMessages = await context.Messages
            .AsNoTracking()
            .Join(context.Conversations,
                message => message.ConversationId,
                conversation => conversation.Id,
                (message, conversation) => new { Message = message, Conversation = conversation })
            .Join(context.Users,
                conversationMessage => conversationMessage.Conversation.SenderUserId,
                user => user.Id,
                (conversationMessage, user) => new { User = user, ConvMessage = conversationMessage })
            .Where(message =>
                message.ConvMessage.Conversation.SenderUserId == userId || message.ConvMessage.Conversation.ReceiverUserId == userId)
            .Where(message => message.ConvMessage.Message.MessageStatus == MessageStatus.Delivered)
            .ToListAsync();

        var messages = await context.Messages
            .AsNoTracking()
            .Include(message => message.Conversation)
            .ThenInclude(conversation => conversation.SenderUser)
            .Include(message => message.Conversation)
            .ThenInclude(conversation => conversation.ReceiverUser)
            .Include(message => message.Attachments)
            .Where(message =>
                message.MessageStatus == MessageStatus.Delivered &&
                (message.Conversation.SenderUserId == userId || message.Conversation.ReceiverUserId == userId))
            .ToListAsync();

        return messages;
    }

    public async Task<int> GetUnreadMessagesCountAsync(string userId)
    {
        return await context.Messages
            .AsNoTracking()
            .Include(message => message.Conversation)
            .ThenInclude(conversation => conversation.SenderUser)
            .Include(message => message.Conversation)
            .ThenInclude(conversation => conversation.ReceiverUser)
            .Where(message =>
                message.Conversation.SenderUserId == userId ||
                message.Conversation.ReceiverUserId == userId)
            .Where(message =>
                message.MessageStatus == MessageStatus.Delivered)
            .CountAsync();
    }

    public async Task<IEnumerable<Message>> SearchMessagesAsync(
        string searchTerm, Guid? conversationId = null)
        => await context.Messages
            .AsNoTracking()
            .Include(message => message.Conversation)
            .ThenInclude(conversation => conversation.SenderUser)
            .Include(message => message.Conversation)
            .ThenInclude(conversation => conversation.ReceiverUser)
            .Include(message => message.Attachments)
            .Where(message =>
                !string.IsNullOrEmpty(message.Content) &&
                message.Content!.ToLower().Contains(searchTerm.ToLower()) &&
                (!conversationId.HasValue || message.ConversationId == conversationId))
            .ToListAsync();
}
