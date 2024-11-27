using Microsoft.AspNetCore.Http;
using Syncify.Application.Interfaces.Services;
using System.Security.Claims;

namespace Syncify.Services.Services;
public class CurrentUser(IHttpContextAccessor contextAccessor) : ICurrentUser
{
    public string Id => contextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
}
