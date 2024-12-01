using Microsoft.AspNetCore.Mvc;
using Syncify.Application.Filters;

namespace Syncify.Application.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class ApiKeyAttribute : ServiceFilterAttribute
{
    public ApiKeyAttribute() : base(typeof(ApiKeyFilter))
    {
        Order = 1;
    }
}
