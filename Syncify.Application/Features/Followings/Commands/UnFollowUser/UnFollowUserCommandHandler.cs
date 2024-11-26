using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Followings.Commands.UnFollowUser;
public sealed class UnFollowUserCommandHandler(IFollowingService followingService) : IRequestHandler<UnFollowUserCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UnFollowUserCommand request,
        CancellationToken cancellationToken)
        => await followingService.UnfollowUserAsync(request.FollowerId, request.FollowedId);
}
