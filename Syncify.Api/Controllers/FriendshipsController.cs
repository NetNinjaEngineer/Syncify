using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Attributes;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Features.FriendRequests.Commands.AcceptFriendRequest;
using Syncify.Application.Features.FriendRequests.Commands.CurrentUserAcceptFriendRequest;
using Syncify.Application.Features.FriendRequests.Commands.CurrentUserSendFriendRequest;
using Syncify.Application.Features.FriendRequests.Commands.SendFriendRequest;
using Syncify.Application.Features.FriendRequests.Queries.AreFriendsForCurrentUser;
using Syncify.Application.Features.FriendRequests.Queries.CheckIfAreFriends;
using Syncify.Application.Features.FriendRequests.Queries.GetLoggedInUserAcceptedFriendships;
using Syncify.Application.Features.FriendRequests.Queries.GetLoggedInUserRequestedFriendships;
using Syncify.Application.Helpers;

namespace Syncify.Api.Controllers;
[Route("api/friendships")]
public class FriendshipsController(IMediator mediator) : ApiBaseController(mediator)
{
    [Guard(roles: [AppConstants.Roles.User])]
    [HttpPost("send")]
    public async Task<ActionResult<Result<FriendshipResponseDto>>> SendFriendshipRequestAsync(SendFriendshipRequestDto request)
        => CustomResult(await Mediator.Send(new SendFriendRequestCommand() { SendFriendshipRequest = request }));

    [Guard(roles: [AppConstants.Roles.User])]
    [HttpPost("{friendRequestId}/accept")]
    public async Task<ActionResult<Result<FriendshipResponseDto>>> AcceptFriendRequestAsync(
        Guid friendRequestId,
        string userId)
    {
        var request = new AcceptFriendRequestDto(userId, friendRequestId);
        return CustomResult(await Mediator.Send(new AcceptFriendRequestCommand { AcceptFriendRequest = request }));
    }

    [Guard(roles: [AppConstants.Roles.User])]
    [HttpGet("currentUser/getRequestedFriendships")]
    public async Task<ActionResult<Result<IEnumerable<PendingFriendshipRequest>>>>
        GetLoggedInUserRequestedFriendshipsAsync()
        => CustomResult(await Mediator.Send(new GetLoggedInUserRequestedFriendshipsQuery()));

    [Guard(roles: [AppConstants.Roles.User])]
    [HttpGet("currentUser/acceptedFriendships")]
    public async Task<ActionResult<Result<IEnumerable<GetUserAcceptedFriendshipDto>>>> GetLoggedInUserAcceptedFriendshipsAsync()
        => CustomResult(await Mediator.Send(new GetLoggedInUserAcceptedFriendshipsQuery()));

    [HttpGet("are-friends")]
    public async Task<ActionResult<Result<bool>>> AreFriendsAsync([FromQuery] CheckIfAreFriendsQuery request)
        => CustomResult(await Mediator.Send(request));

    [Guard(roles: [AppConstants.Roles.User])]
    [HttpGet("currentUser/are-friends")]
    public async Task<ActionResult<Result<bool>>> AreFriendsWithCurrentUserAsync([FromQuery] AreFriendsForCurrentUserQuery request)
        => CustomResult(await Mediator.Send(request));

    [Guard(roles: [AppConstants.Roles.User])]
    [HttpPost("currentUser/send-friendRequest")]
    public async Task<ActionResult<Result<FriendshipResponseDto>>> SendFriendshipCurrentUserAsync([FromQuery] CurrentUserSendFriendRequestCommand command)
        => CustomResult(await Mediator.Send(command));

    [Guard(roles: [AppConstants.Roles.User])]
    [HttpPost("{friendRequestId}/currentUser/accept")]
    public async Task<ActionResult<Result<FriendshipResponseDto>>> AcceptFriendRequestAsync(Guid friendRequestId) =>
        CustomResult(await Mediator.Send(
            new CurrentUserAcceptFriendRequestCommand
            {
                FriendshipId = friendRequestId
            }));
}
