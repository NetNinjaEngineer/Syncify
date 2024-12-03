using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Messages.Queries.GetUnreadMessagesCount;
public sealed class GetUnreadMessagesCountQuery : IRequest<Result<int>>
{
    public required string UserId { get; set; }
}
