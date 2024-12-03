using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Queries.GetUnreadMessages;
public sealed class GetUnreadMessagesQueryHandler(IMessageService service) : IRequestHandler<GetUnreadMessagesQuery, Result<IEnumerable<MessageDto>>>
{
    public async Task<Result<IEnumerable<MessageDto>>> Handle(
        GetUnreadMessagesQuery request, CancellationToken cancellationToken)
    {
        return await service.GetUnreadMessagesAsync(request);
    }
}
