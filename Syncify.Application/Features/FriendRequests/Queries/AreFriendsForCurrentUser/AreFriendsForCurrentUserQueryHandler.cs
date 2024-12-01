using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.FriendRequests.Queries.AreFriendsForCurrentUser;
public sealed class AreFriendsForCurrentUserQueryHandler(IFriendshipService friendshipService)
    : IRequestHandler<AreFriendsForCurrentUserQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(AreFriendsForCurrentUserQuery request, CancellationToken cancellationToken)
        => await friendshipService.AreFriendsWithCurrentUserAsync(request);
}
