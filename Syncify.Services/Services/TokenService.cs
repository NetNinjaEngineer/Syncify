using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Syncify.Services.Services;
public sealed class TokenService(
    UserManager<ApplicationUser> userManager,
    IOptions<JwtSettings> jwtOptions,
    IConfiguration configuration,
    IHttpContextAccessor contextAccessor) : ITokenService
{
    private readonly JwtSettings _jwtSettings = jwtOptions.Value;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    public async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
    {
        var userClaims = await userManager.GetClaimsAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

        var ipAddress = contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var userAgent = contextAccessor.HttpContext?.Request.Headers.UserAgent.ToString();
        var fingerPrint = Convert.ToBase64String(
            SHA256.HashData(Encoding.UTF8.GetBytes(string.Concat(ipAddress, userAgent))));

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(CustomClaims.FullName, $"{user.FirstName} {user.LastName}".Trim()),
            new Claim(CustomClaims.DateOfBirth, user.DateOfBirth.ToString("yyyy-MM-dd")),
            new Claim(CustomClaims.Gender, user.Gender.ToString()),
            new Claim(CustomClaims.ProfilePictureUrl, $"{configuration["BaseApiUrl"]}/Uploads/Images/{user.ProfilePictureUrl}" ?? string.Empty),
            new Claim(CustomClaims.CreatedAt, user.CreatedAt.ToString("O")),
            new Claim(CustomClaims.Bio, user.Bio ?? string.Empty),
            new Claim(CustomClaims.Uid, user.Id),
            new Claim(CustomClaims.IP, ipAddress!),
            new Claim(CustomClaims.UserAgent, userAgent!),
            new Claim(CustomClaims.FingerPrint, fingerPrint)
        }
            .Union(userClaims)
            .Union(roleClaims);

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTimeOffset.Now.AddDays(_jwtSettings.ExpirationInDays).UtcDateTime,
            signingCredentials: signingCredentials);

        return _jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
    }

    public Task<ClaimsPrincipal> ValidateToken(string token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        var principal = _jwtSecurityTokenHandler.ValidateToken(
            token,
            validationParameters,
            out _
        );
        return Task.FromResult(principal);
    }
}
