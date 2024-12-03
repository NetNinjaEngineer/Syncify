using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Queries.GetUnreadMessagesCount;
public sealed class GetUnreadMessagesCountQueryHandler(IMessageService service) :
    IRequestHandler<GetUnreadMessagesCountQuery, Result<int>>
{
    public async Task<Result<int>> Handle(
        GetUnreadMessagesCountQuery request, CancellationToken cancellationToken)
        => await service.GetUnreadMessageCountAsync(request);
}
