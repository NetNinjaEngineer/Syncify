using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;

namespace Syncify.Application.Features.FriendRequests.Queries.GetLoggedInUserAcceptedFriendships;
public sealed class GetLoggedInUserAcceptedFriendshipsQuery : IRequest<Result<IEnumerable<GetUserAcceptedFriendshipDto>>>
{
}
