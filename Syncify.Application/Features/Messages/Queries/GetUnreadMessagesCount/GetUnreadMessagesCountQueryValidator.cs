using FluentValidation;

namespace Syncify.Application.Features.Messages.Queries.GetUnreadMessagesCount;

public sealed class GetUnreadMessagesCountQueryValidator : AbstractValidator<GetUnreadMessagesCountQuery>
{
    public GetUnreadMessagesCountQueryValidator()
    {
        RuleFor(x => x.UserId)
          .NotNull().WithMessage("{PropertyName} can not be null.")
          .NotEmpty().WithMessage("{PropertyName} can not be empty.");
    }
}