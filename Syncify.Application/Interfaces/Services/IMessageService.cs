using Syncify.Application.Bases;
using Syncify.Application.DTOs.Conversation;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Features.Conversations.Queries.GetPagedConversationMessages;
using Syncify.Application.Features.Conversations.Queries.GetUserConversation;
using Syncify.Application.Features.Messages.Commands.DeleteMessageInConversation;
using Syncify.Application.Features.Messages.Commands.MarkMessageAsRead;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessage;
using Syncify.Application.Features.Messages.Commands.SendPrivateMessageByCurrentUser;
using Syncify.Application.Features.Messages.Queries.GetMessagesByDateRange;
using Syncify.Application.Features.Messages.Queries.GetUnreadMessages;
using Syncify.Application.Features.Messages.Queries.GetUnreadMessagesCount;
using Syncify.Application.Features.Messages.Queries.SearchMessages;

namespace Syncify.Application.Interfaces.Services;
public interface IMessageService
{
    Task<Result<MessageDto>> SendPrivateMessageAsync(SendPrivateMessageCommand command);
    Task<Result<MessageDto>> SendPrivateMessageByCurrentUserAsync(SendPrivateMessageByCurrentUserCommand command);
    // Retrieve message methods
    Task<Result<ConversationDto>> GetConversationMessagesAsync(GetPagedConversationMessagesQuery query);
    Task<Result<ConversationDto>> GetConversationMessagesAsync(Guid conversationId);
    Task<Result<ConversationDto>> GetUserConversationsAsync(GetUserConversationQuery query);
    Task<Result<MessageDto>> GetMessageByIdAsync(Guid messageId);

    // Message status and management methods
    Task<Result<bool>> MarkMessageAsReadAsync(MarkMessageAsReadCommand command);
    Task<Result<int>> GetUnreadMessageCountAsync(GetUnreadMessagesCountQuery query);
    Task<Result<IEnumerable<MessageDto>>> GetUnreadMessagesAsync(GetUnreadMessagesQuery query);
    Task<Result<bool>> DeleteMessageAsync(Guid messageId);
    Task<Result<bool>> DeleteMessageInConversationAsync(DeleteMessageInConversationCommand command);
    Task<Result<bool>> EditMessageAsync(Guid messageId, string newContent);

    // Group chat methods
    //Task<ConversationDto> CreateGroupConversationAsync(CreateGroupConversationCommand command);
    //Task<bool> AddUserToGroupAsync(Guid conversationId, string userId);
    //Task<bool> RemoveUserFromGroupAsync(Guid conversationId, string userId);
    //Task<MessageDto> SendGroupMessageAsync(SendGroupMessageCommand command);

    // Real-time and notification methods
    //Task<bool> BlockUserAsync(string blockedUserId);
    //Task<bool> UnblockUserAsync(string blockedUserId);
    //Task<IEnumerable<UserDto>> GetBlockedUsersAsync();

    // Search and filtering methods
    Task<Result<IEnumerable<MessageDto>>> SearchMessagesAsync(SearchMessagesQuery searchMessagesQuery);
    Task<Result<IEnumerable<MessageDto>>> GetMessagesByDateRangeAsync(GetMessagesByDateRangeQuery query);
}
