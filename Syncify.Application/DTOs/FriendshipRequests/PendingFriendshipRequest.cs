namespace Syncify.Application.DTOs.FriendshipRequests;
public sealed class PendingFriendshipRequest
{
    public string RequesterId { get; set; } = string.Empty;
    public string RequesterName { get; set; } = string.Empty;
    public string ReceiverId { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; set; }
}
