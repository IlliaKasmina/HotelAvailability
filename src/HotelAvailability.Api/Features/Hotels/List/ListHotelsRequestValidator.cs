using FluentValidation;

namespace HotelAvailability.Api.Features.Hotels.List;

/// <summary>Validates the optional pagination parameters for the hotels list.</summary>
public sealed class ListHotelsRequestValidator : AbstractValidator<ListHotelsRequest>
{
    public ListHotelsRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be 1 or greater.")
            .When(x => x.Page.HasValue);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.")
            .When(x => x.PageSize.HasValue);
    }
}
