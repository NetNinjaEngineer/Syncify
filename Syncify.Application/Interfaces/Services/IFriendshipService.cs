using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Features.FriendRequests.Commands.CurrentUserAcceptFriendRequest;
using Syncify.Application.Features.FriendRequests.Commands.CurrentUserSendFriendRequest;
using Syncify.Application.Features.FriendRequests.Queries.AreFriendsForCurrentUser;
using Syncify.Application.Features.FriendRequests.Queries.CheckIfAreFriends;

namespace Syncify.Application.Interfaces.Services;
public interface IFriendshipService
{
    Task<Result<FriendshipResponseDto>> SendFriendRequestAsync(SendFriendshipRequestDto sendFriendshipRequest);

    Task<Result<FriendshipResponseDto>> AcceptFriendRequestAsync(AcceptFriendRequestDto acceptFriendRequest);

    Task<Result<IEnumerable<PendingFriendshipRequest>>> GetLoggedInUserRequestedFriendshipsAsync();

    Task<Result<IEnumerable<GetUserAcceptedFriendshipDto>>> GetLoggedInUserAcceptedFriendshipsAsync();

    Task<Result<bool>> AreFriendsAsync(CheckIfAreFriendsQuery query);

    Task<Result<bool>> AreFriendsWithCurrentUserAsync(AreFriendsForCurrentUserQuery query);

    Task<Result<FriendshipResponseDto>> SendFriendRequestCurrentUserAsync(CurrentUserSendFriendRequestCommand command);

    Task<Result<FriendshipResponseDto>> CurrentUserAcceptFriendRequestAsync(CurrentUserAcceptFriendRequestCommand command);

}
