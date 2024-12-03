using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Messages;

namespace Syncify.Application.Features.Messages.Queries.GetMessagesByDateRange;
public sealed class GetMessagesByDateRangeQuery : IRequest<Result<IEnumerable<MessageDto>>>
{
    public required DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public Guid? ConversationId { get; set; } = null;
}
