using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Messages.Commands.DeleteMessage;
public sealed class DeleteMessageCommand : IRequest<Result<bool>>
{
    public Guid MessageId { get; set; }
}
