using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;

namespace Syncify.Application.Interfaces.Services;
public interface IMessageService
{
    Task<Result<MessageDto>> SendPrivateMessageAsync(SendPrivateMessageCommand command);
    Task<Result<MessageDto>> SendPrivateMessageByCurrentUserAsync(SendPrivateMessageByCurrentUserCommand command);
    // Retrieve message methods
    Task<Result<ConversationDto>> GetConversationMessagesAsync(Guid conversationId, int pageNumber, int pageSize);
    Task<Result<ConversationDto>> GetConversationMessagesAsync(Guid conversationId);
    Task<Result<ConversationDto>> GetUserConversationsAsync(string userId);
    Task<Result<MessageDto>> GetMessageByIdAsync(Guid messageId);

    // Message status and management methods
    Task<Result<bool>> MarkMessageAsReadAsync(Guid messageId);
    Task<Result<int>> GetUnreadMessageCountAsync(string userId);
    Task<Result<bool>> DeleteMessageAsync(Guid messageId);
    Task<Result<bool>> EditMessageAsync(Guid messageId, string newContent);

    // Group chat methods
    //Task<ConversationDto> CreateGroupConversationAsync(CreateGroupConversationCommand command);
    //Task<bool> AddUserToGroupAsync(Guid conversationId, string userId);
    //Task<bool> RemoveUserFromGroupAsync(Guid conversationId, string userId);
    //Task<MessageDto> SendGroupMessageAsync(SendGroupMessageCommand command);

    // Media and attachment methods
    //Task<MessageDto> SendMediaMessageAsync(SendMediaMessageCommand command);
    //Task<IEnumerable<MediaDto>> GetConversationMediaAsync(Guid conversationId);

    // Real-time and notification methods
    //Task<bool> BlockUserAsync(string blockedUserId);
    //Task<bool> UnblockUserAsync(string blockedUserId);
    //Task<IEnumerable<UserDto>> GetBlockedUsersAsync();

    // Search and filtering methods
    Task<Result<IEnumerable<MessageDto>>> SearchMessagesAsync(string searchTerm, Guid? conversationId = null);
    Task<Result<IEnumerable<MessageDto>>> GetMessagesByDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate, Guid? conversationId = null);
}
