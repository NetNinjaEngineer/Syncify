using Syncify.Application.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace Syncify.Api.Middleware;

public sealed class JwtValidationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var jwtToken = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);

        if (!string.IsNullOrEmpty(jwtToken))
        {
            // check for each request if the token is used from different location or any other device
            // using ip address and user-agent
            // check fingerprint based on ip address and user-agent

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(jwtToken);

            var ipAddressClaim = securityToken.Claims?.FirstOrDefault(c => c.Type == CustomClaims.IP)?.Value;
            var userAgentClaim = securityToken.Claims?.FirstOrDefault(c => c.Type == CustomClaims.UserAgent)?.Value;
            var fingerPrintClaim = securityToken.Claims?.FirstOrDefault(c => c.Type == CustomClaims.FingerPrint)?.Value;

            var currentIpAddress = context.Connection.RemoteIpAddress?.ToString();
            var currentUserAgent = context.Request.Headers["User-Agent"].ToString();
            var currentFingerPrint = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(string.Concat(currentIpAddress, currentUserAgent))));

            if (ipAddressClaim != currentIpAddress || userAgentClaim != currentUserAgent
                || fingerPrintClaim != currentFingerPrint)
            {
                context.Response.StatusCode = 401;

                await context.Response.WriteAsJsonAsync(
                    new GlobalErrorResponse
                    {
                        Type = "Unauthorized",
                        Status = StatusCodes.Status401Unauthorized,
                        Message = "You are not authorized to perform this action."
                    });

                return;
            }

        }

        await next(context);
    }
}
