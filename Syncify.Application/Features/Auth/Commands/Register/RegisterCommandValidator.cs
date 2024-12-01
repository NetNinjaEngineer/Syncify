﻿using FluentValidation;

namespace Syncify.Application.Features.Auth.Commands.Register;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email cannot be null.")
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email address.");

        RuleFor(x => x.FirstName)
            .NotNull().WithMessage("FirstName cannot be null.")
            .NotEmpty().WithMessage("FirstName is required")
            .MinimumLength(3).WithMessage("FirstName must be at least 3 characters.")
            .MaximumLength(50).WithMessage("FirstName must be between 3 and 50 characters.");

        RuleFor(x => x.LastName)
            .NotNull().WithMessage("LastName cannot be null.")
            .NotEmpty().WithMessage("LastName is required")
            .MinimumLength(3).WithMessage("LastName must be at least 3 characters.")
            .MaximumLength(50).WithMessage("LastName must be between 3 and 50 characters.");

        RuleFor(x => x.UserName)
            .NotNull().WithMessage("UserName cannot be null.")
            .NotEmpty().WithMessage("UserName is required");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("Password cannot be null.")
            .NotEmpty().WithMessage("Password is required")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$")
            .WithMessage("Password must be at least 8 characters long, include at least one lowercase letter, one uppercase letter, one digit, and one special character (e.g., !@#$%^&*).")
            .Equal(x => x.ConfirmPassword)
            .WithMessage("Passwords do not match.");
    }
}