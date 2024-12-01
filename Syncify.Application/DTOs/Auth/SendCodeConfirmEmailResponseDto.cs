namespace Syncify.Application.DTOs.Auth;
public sealed record SendCodeConfirmEmailResponseDto
{
    public DateTimeOffset CodeExpiration { get; set; }

    public static SendCodeConfirmEmailResponseDto ToResponse(DateTimeOffset expiration)
        => new() { CodeExpiration = expiration };

}

