﻿using System.Text.Json.Serialization;

namespace Syncify.Application.DTOs.Auth;
public sealed record SignInResponseDto
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = [];
    public bool IsAuthenticated { get; set; }
    public DateTimeOffset RefreshTokenExpiration { get; set; }
    [JsonIgnore] public string? RefreshToken { get; set; }
}