using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;

namespace Syncify.Application.Features.Messages.Queries.SearchMessages;
public sealed class SearchMessagesQuery : IRequest<Result<IEnumerable<MessageDto>>>
{
    public required string SearchTerm { get; set; }
    public Guid? ConversationId { get; set; } = null;
}
