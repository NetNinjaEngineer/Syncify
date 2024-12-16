using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Filters;

namespace Syncify.Api.Attributes;
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class GuardAttribute : TypeFilterAttribute
{
    public GuardAttribute(
        string[]? policies = null,
        string[]? roles = null) : base(typeof(GuardFilter))
    {
        Arguments = [policies ?? [], roles ?? []];
        Order = 2;
    }
}
