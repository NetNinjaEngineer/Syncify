namespace Syncify.Application.DTOs.FriendshipRequests;

public sealed class GetUserAcceptedFriendshipDto
{
    public string FriendName { get; set; } = null!;
    public DateTimeOffset AcceptedAt { get; set; }
}