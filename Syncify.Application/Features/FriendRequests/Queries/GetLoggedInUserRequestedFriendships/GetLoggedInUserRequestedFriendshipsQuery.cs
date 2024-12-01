using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;

namespace Syncify.Application.Features.FriendRequests.Queries.GetLoggedInUserRequestedFriendships;
public sealed class GetLoggedInUserRequestedFriendshipsQuery : IRequest<Result<IEnumerable<PendingFriendshipRequest>>>
{
}