namespace Syncify.Application.DTOs.FriendshipRequests;

public sealed record FriendshipResponseDto(
    string Requester,
    string Receiver,
    string FriendshipStatus,
    DateTimeOffset CreatedAt,
    DateTimeOffset? UpdatedAt
);
