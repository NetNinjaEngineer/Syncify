﻿namespace Syncify.Application.DTOs.Auth;
public sealed record ValidateTokenResponseDto
{
    public List<ClaimsResponse> Claims { get; set; } = [];
}