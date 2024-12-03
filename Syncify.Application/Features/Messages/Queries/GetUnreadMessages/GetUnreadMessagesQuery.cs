using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;

namespace Syncify.Application.Features.Messages.Queries.GetUnreadMessages;
public sealed class GetUnreadMessagesQuery : IRequest<Result<IEnumerable<MessageDto>>>
{
    public required string UserId { get; set; }
}
