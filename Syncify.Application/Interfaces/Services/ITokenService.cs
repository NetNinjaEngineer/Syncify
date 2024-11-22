using Syncify.Domain.Entities.Identity;

namespace Syncify.Application.Interfaces.Services;
public interface ITokenService
{
    Task<string> GenerateJwtTokenAsync(ApplicationUser user);
}
