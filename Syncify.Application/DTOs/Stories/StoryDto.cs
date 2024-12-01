namespace Syncify.Application.DTOs.Stories;
public sealed class StoryDto
{
    public string UserId { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    public string? Caption { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
}
