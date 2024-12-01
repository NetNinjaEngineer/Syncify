﻿using MediatR;
using Syncify.Application.Bases;
using Syncify.Application.Interfaces.Services;

namespace Syncify.Application.Features.Auth.Commands.Disable2Fa;
public sealed class Disable2FaCommandHandler(IAuthService authService)
    : IRequestHandler<Disable2FaCommand, Result<string>>
{
    public async Task<Result<string>> Handle(Disable2FaCommand request, CancellationToken cancellationToken)
        => await authService.Disable2FaAsync(request);
}