using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.FriendRequests.Queries.CheckIfAreFriends;
public sealed class CheckIfAreFriendsQuery : IRequest<Result<bool>>
{
    public string User1Id { get; set; } = null!;
    public string User2Id { get; set; } = null!;
}
