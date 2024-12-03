using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Queries.SearchMessages;
public sealed class SearchMessagesQueryHandler(
    IMessageService service) : IRequestHandler<SearchMessagesQuery, Result<IEnumerable<MessageDto>>>
{
    public async Task<Result<IEnumerable<MessageDto>>> Handle(
        SearchMessagesQuery request,
        CancellationToken cancellationToken)
        => await service.SearchMessagesAsync(request);
}
