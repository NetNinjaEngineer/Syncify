using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.FriendRequests.Queries.CheckIfAreFriends;
public sealed class CheckIfAreFriendsQueryHandler(IFriendshipService friendshipService) : IRequestHandler<CheckIfAreFriendsQuery, Result<bool>>
{
    public async Task<Result<bool>> Handle(CheckIfAreFriendsQuery request, CancellationToken cancellationToken)
        => await friendshipService.AreFriendsAsync(request);
}
