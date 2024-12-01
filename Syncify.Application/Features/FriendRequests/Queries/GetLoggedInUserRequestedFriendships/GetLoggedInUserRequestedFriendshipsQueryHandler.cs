using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.FriendRequests.Queries.GetLoggedInUserRequestedFriendships;

public sealed class GetLoggedInUserRequestedFriendshipsQueryHandler(IFriendshipService friendshipService)
    : IRequestHandler<GetLoggedInUserRequestedFriendshipsQuery, Result<IEnumerable<PendingFriendshipRequest>>>
{
    public async Task<Result<IEnumerable<PendingFriendshipRequest>>> Handle(
        GetLoggedInUserRequestedFriendshipsQuery request,
        CancellationToken cancellationToken)
        => await friendshipService.GetLoggedInUserRequestedFriendshipsAsync();
}