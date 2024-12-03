using FluentValidation;

namespace Syncify.Application.Features.Messages.Queries.GetUnreadMessages;

public sealed class GetUnreadMessagesQueryValidator : AbstractValidator<GetUnreadMessagesQuery>
{
    public GetUnreadMessagesQueryValidator()
    {
        RuleFor(x => x.UserId)
            .NotNull().WithMessage("{PropertyName} can not be null.")
            .NotEmpty().WithMessage("{PropertyName} can not be empty.");
    }
}