public interface IMessageService
{
    // Existing methods
    Task<MessageDto> SendPrivateMessageAsync(SendPrivateMessageCommand command);
    Task<MessageDto> SendPrivateMessageByCurrentUserAsync(SendPrivateMessageByCurrentUserCommand command);

    // Retrieve message methods
    Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(Guid conversationId, int pageNumber, int pageSize);
    Task<IEnumerable<MessageDto>> GetUserConversationsAsync(string userId);
    Task<MessageDto> GetMessageByIdAsync(Guid messageId);

    // Message status and management methods
    Task<bool> MarkMessageAsReadAsync(Guid messageId);
    Task<int> GetUnreadMessageCountAsync(string userId);
    Task<bool> DeleteMessageAsync(Guid messageId);
    Task<bool> EditMessageAsync(Guid messageId, string newContent);

    // Group chat methods
    Task<ConversationDto> CreateGroupConversationAsync(CreateGroupConversationCommand command);
    Task<bool> AddUserToGroupAsync(Guid conversationId, string userId);
    Task<bool> RemoveUserFromGroupAsync(Guid conversationId, string userId);
    Task<MessageDto> SendGroupMessageAsync(SendGroupMessageCommand command);

    // Media and attachment methods
    Task<MessageDto> SendMediaMessageAsync(SendMediaMessageCommand command);
    Task<IEnumerable<MediaDto>> GetConversationMediaAsync(Guid conversationId);

    // Real-time and notification methods
    Task<bool> BlockUserAsync(string blockedUserId);
    Task<bool> UnblockUserAsync(string blockedUserId);
    Task<IEnumerable<UserDto>> GetBlockedUsersAsync();

    // Search and filtering methods
    Task<IEnumerable<MessageDto>> SearchMessagesAsync(string searchTerm, Guid? conversationId = null);
    Task<IEnumerable<MessageDto>> GetMessagesByDateRangeAsync(DateTime startDate, DateTime endDate, Guid? conversationId = null);
}

// Supporting DTOs and Command classes
public class SendPrivateMessageCommand
{
    public string SenderId { get; set; }
    public string ReceiverId { get; set; }
    public string Content { get; set; }
    public List<AttachmentDto> Attachments { get; set; }
}

public class SendGroupMessageCommand
{
    public string SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public List<AttachmentDto> Attachments { get; set; }
}

public class CreateGroupConversationCommand
{
    public string CreatorUserId { get; set; }
    public string GroupName { get; set; }
    public List<string> InitialMemberIds { get; set; }
}

public class MessageDto
{
    public Guid Id { get; set; }
    public string SenderId { get; set; }
    public Guid ConversationId { get; set; }
    public string Content { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsRead { get; set; }
    public List<AttachmentDto> Attachments { get; set; }
}

public class ConversationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsGroupConversation { get; set; }
    public List<string> MemberIds { get; set; }
    public MessageDto LastMessage { get; set; }
}

public class AttachmentDto
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string FileType { get; set; }
    public string FileUrl { get; set; }
    public long FileSize { get; set; }
}

public class MediaDto : AttachmentDto
{
    public string ThumbnailUrl { get; set; }
}
