using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Queries.GetMessagesByDateRange;
public sealed class GetMessagesByDateRangeQueryHandler(IMessageService service)
    : IRequestHandler<GetMessagesByDateRangeQuery, Result<IEnumerable<MessageDto>>>
{
    public async Task<Result<IEnumerable<MessageDto>>> Handle(GetMessagesByDateRangeQuery request,
        CancellationToken cancellationToken)
        => await service.GetMessagesByDateRangeAsync(request);
}
