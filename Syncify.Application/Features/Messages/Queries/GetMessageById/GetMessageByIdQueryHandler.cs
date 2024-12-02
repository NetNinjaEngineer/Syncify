using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Messages.Queries.GetMessageById;
public sealed class GetMessageByIdQueryHandler(
    IMessageService messageService) : IRequestHandler<GetMessageByIdQuery, Result<MessageDto>>
{
    public async Task<Result<MessageDto>> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
        => await messageService.GetMessageByIdAsync(request.MessageId);
}
