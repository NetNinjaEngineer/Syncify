using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;

namespace Syncify.Application.Features.Messages.Queries.GetMessageById;
public sealed class GetMessageByIdQuery : IRequest<Result<MessageDto>>
{
    public Guid MessageId { get; set; }
}
