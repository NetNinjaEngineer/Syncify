using FluentValidation;

namespace Syncify.Application.Features.Auth.Commands.SendConfirmEmailCode;
public sealed class SendConfirmEmailCodeCommandValidator : AbstractValidator<SendConfirmEmailCodeCommand>
{
    public SendConfirmEmailCodeCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email cannot be null.")
            .NotEmpty().WithMessage("Email cannot be empty.")
            .EmailAddress().WithMessage("Invalid email address.");
    }
}
