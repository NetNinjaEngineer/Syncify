using FluentValidation;

namespace Syncify.Application.Features.Auth.Commands.SignInGoogle;
public sealed class SignInGoogleCommandValidator : AbstractValidator<SignInGoogleCommand>
{
    public SignInGoogleCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("{PropertyName} can not be empty.")
            .Must(BeValidJwtFormat).WithMessage("Token must be in valid JWT format");
    }

    private static bool BeValidJwtFormat(string token) => token.Split(".").Length == 3;
}
