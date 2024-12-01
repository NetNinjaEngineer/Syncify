using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.FriendRequests.Queries.AreFriendsForCurrentUser;
public sealed class AreFriendsForCurrentUserQuery : IRequest<Result<bool>>
{
    public string FriendId { get; set; } = null!;
}
