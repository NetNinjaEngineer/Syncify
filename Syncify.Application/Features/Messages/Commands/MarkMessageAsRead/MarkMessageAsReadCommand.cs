using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Messages.Commands.MarkMessageAsRead;
public sealed class MarkMessageAsReadCommand : IRequest<Result<bool>>
{
    public Guid MessageId { get; set; }
}
