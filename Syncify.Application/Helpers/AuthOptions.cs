namespace Syncify.Application.Helpers;
public sealed class AuthOptions
{
    public GoogleOptions GoogleOptions { get; set; } = new();
    public FacebookOptions FacebookOptions { get; set; } = new();
}