using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Followings.Commands.UnFollowUser;
public sealed class UnFollowUserCommand : IRequest<Result<bool>>
{
    public required string FollowerId { get; set; }
    public required string FollowedId { get; set; }
}
