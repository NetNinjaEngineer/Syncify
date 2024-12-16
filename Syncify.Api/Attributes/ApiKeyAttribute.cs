using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Filters;

namespace Syncify.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAttribute : ServiceFilterAttribute
{
    public ApiKeyAttribute() : base(typeof(ApiKeyFilter))
    {
        Order = 1;
    }
}
