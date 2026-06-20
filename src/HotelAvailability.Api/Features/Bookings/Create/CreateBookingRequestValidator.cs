using FluentValidation;

namespace HotelAvailability.Api.Features.Bookings.Create;

/// <summary>
/// Validates a booking request so the (not-yet-implemented) endpoint still rejects malformed
/// input with a 400 before reaching the 501 stub.
/// </summary>
public sealed class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.HotelId).NotEmpty();
        RuleFor(x => x.RoomId).NotEmpty();
        RuleFor(x => x.RatePlanId).NotEmpty();
        RuleFor(x => x.Rooms).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Adults).GreaterThanOrEqualTo(1);

        RuleFor(x => x.CheckOut)
            .GreaterThan(x => x.CheckIn).WithMessage("Check-out must be after check-in.");

        RuleForEach(x => x.ChildrenAges)
            .InclusiveBetween(0, 17).WithMessage("Each child's age must be between 0 and 17.")
            .When(x => x.ChildrenAges is not null);

        RuleFor(x => x.Lead).NotNull();
        RuleFor(x => x.Lead.FirstName).NotEmpty().When(x => x.Lead is not null);
        RuleFor(x => x.Lead.LastName).NotEmpty().When(x => x.Lead is not null);
        RuleFor(x => x.Lead.Email).NotEmpty().EmailAddress().When(x => x.Lead is not null);
    }
}
