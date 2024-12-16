using Microsoft.AspNetCore.Mvc;

namespace Syncify.Api;
public sealed class GlobalErrorResponse : ProblemDetails
{
    public string? Message { get; set; }
    public IEnumerable<string> Errors { get; set; } = [];
}
