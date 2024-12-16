using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Syncify.Api.Filters;
public sealed class ApiKeyFilter(IConfiguration configuration) : IAsyncAuthorizationFilter
{
    private const string ApiKeyHeaderName = "X-API-Key";

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];

        if (!CheckIfValidApiKey(apiKey))
        {
            context.Result = new UnauthorizedObjectResult(
                new GlobalErrorResponse
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Missing Api Key",
                    Detail = "API Key required to access the endpoints, API Key is sent as a request header 'X-API-Key' .",
                    Type = "Unauthorized_Error"
                });
        }

        return Task.CompletedTask;
    }

    private bool CheckIfValidApiKey(string? apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
            return false;

        return apiKey == configuration["ApiKey"];
    }
}
