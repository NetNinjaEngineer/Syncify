using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.FriendRequests.Queries.GetLoggedInUserAcceptedFriendships;
public sealed class GetLoggedInUserAcceptedFriendshipsQueryHandler(IFriendshipService friendshipService)
    : IRequestHandler<GetLoggedInUserAcceptedFriendshipsQuery, Result<IEnumerable<GetUserAcceptedFriendshipDto>>>
{
    public async Task<Result<IEnumerable<GetUserAcceptedFriendshipDto>>> Handle(
        GetLoggedInUserAcceptedFriendshipsQuery request,
        CancellationToken cancellationToken)
        => await friendshipService.GetLoggedInUserAcceptedFriendshipsAsync();
}
