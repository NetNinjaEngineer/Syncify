using MediatR;
using Microsoft.AspNetCore.Mvc;
using Syncify.Api.Base;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.Auth;
using Syncify.Application.Features.Auth.Commands.SignInGoogle;
using Syncify.Application.Helpers;

namespace Syncify.Api.Controllers;

[Route("api/auth")]
public class AuthController(IMediator mediator) : ApiBaseController(mediator)
{
    [HttpPost("signin-google")]
    [ProducesResponseType(typeof(Result<GoogleUserProfile?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(GlobalErrorResponse), StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(typeof(Result<string>), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<GoogleUserProfile>>> SigninWithGoogleAsync(SignInGoogleCommand command)
        => CustomResult(await Mediator.Send(command));
}
