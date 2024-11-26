using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;

namespace Syncify.Application.Interfaces.Services;
public interface IFriendshipService
{
    Task<Result<FriendshipResponseDto>> SendFriendRequestAsync(SendFriendshipRequestDto sendFriendshipRequest);

    Task<Result<FriendshipResponseDto>> AcceptFriendRequestAsync(AcceptFriendRequestDto acceptFriendRequest);
}
