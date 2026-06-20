using FluentValidation;

namespace HotelAvailability.Api.Features.Availability.Search;

/// <summary>
/// Validates the availability search, including the three stay-window business rules.
/// <see cref="TimeProvider"/> is injected (rather than reading <c>DateTime.Now</c>) so the
/// date rules are deterministic and unit-testable.
/// </summary>
public sealed class SearchAvailabilityRequestValidator : AbstractValidator<SearchAvailabilityRequest>
{
    public SearchAvailabilityRequestValidator(TimeProvider timeProvider)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetUtcNow().UtcDateTime);
        var latestCheckIn = today.AddYears(1);

        RuleFor(x => x.HotelId)
            .NotEmpty().WithMessage("Hotel identifier is required.");

        RuleFor(x => x.Rooms)
            .GreaterThanOrEqualTo(1).WithMessage("At least one room is required.");

        RuleFor(x => x.Adults)
            .GreaterThanOrEqualTo(1).WithMessage("At least one adult is required.");

        RuleFor(x => x.CheckIn)
            .GreaterThanOrEqualTo(today)
                .WithMessage("A stay may start today at the earliest.")
            .LessThanOrEqualTo(latestCheckIn)
                .WithMessage("A stay may be created at most one year in advance.");

        RuleFor(x => x.CheckOut)
            .GreaterThan(x => x.CheckIn)
                .WithMessage("Check-out must be after check-in.");

        // Maximum length of stay is one month. Only meaningful once check-out is after check-in.
        RuleFor(x => x.CheckOut)
            .Must((request, checkOut) => checkOut <= request.CheckIn.AddMonths(1))
                .WithMessage("Maximum length of stay is one month.")
            .When(x => x.CheckOut > x.CheckIn);

        // Children ages are only present when the booking includes children; each must be 0-17.
        RuleForEach(x => x.ChildrenAges)
            .InclusiveBetween(0, 17).WithMessage("Each child's age must be between 0 and 17.")
            .When(x => x.ChildrenAges is not null);
    }
}
