namespace Syncify.Application.DTOs.FriendshipRequests;

public sealed record AcceptFriendRequestDto(
    string UserId,
    Guid FriendshipId
);
