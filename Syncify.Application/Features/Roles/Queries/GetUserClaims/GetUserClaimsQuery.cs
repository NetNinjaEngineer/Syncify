using MediatR;
using Syncify.Application.Bases;

namespace Syncify.Application.Features.Roles.Queries.GetUserClaims;

public sealed class GetUserClaimsQuery : IRequest<Result<IEnumerable<string>>>
{
    public string UserId { get; set; } = string.Empty;
}