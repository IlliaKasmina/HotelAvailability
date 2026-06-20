using FluentValidation.TestHelper;
using HotelAvailability.Api.Features.Availability.Search;
using Microsoft.Extensions.Time.Testing;

namespace HotelAvailability.Tests;

public sealed class SearchAvailabilityRequestValidatorTests
{
    // Fixed "now" so the date rules are deterministic.
    private static readonly DateOnly Today = new(2026, 6, 19);

    private static SearchAvailabilityRequestValidator CreateValidator()
    {
        var timeProvider = new FakeTimeProvider(new DateTimeOffset(Today, new TimeOnly(9, 0), TimeSpan.Zero));
        return new SearchAvailabilityRequestValidator(timeProvider);
    }

    private static SearchAvailabilityRequest ValidRequest() => new()
    {
        HotelId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        CheckIn = Today.AddDays(10),
        CheckOut = Today.AddDays(13),
        Rooms = 1,
        Adults = 2,
        ChildrenAges = null,
    };

    [Fact]
    public void Valid_request_passes()
    {
        var result = CreateValidator().TestValidate(ValidRequest());
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Stay_may_start_today()
    {
        var request = ValidRequest() with { CheckIn = Today, CheckOut = Today.AddDays(2) };
        var result = CreateValidator().TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.CheckIn);
    }

    [Fact]
    public void Stay_cannot_start_in_the_past()
    {
        var request = ValidRequest() with { CheckIn = Today.AddDays(-1), CheckOut = Today.AddDays(2) };
        var result = CreateValidator().TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CheckIn)
            .WithErrorMessage("A stay may start today at the earliest.");
    }

    [Fact]
    public void Stay_cannot_start_more_than_one_year_ahead()
    {
        var checkIn = Today.AddYears(1).AddDays(1);
        var request = ValidRequest() with { CheckIn = checkIn, CheckOut = checkIn.AddDays(2) };
        var result = CreateValidator().TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CheckIn)
            .WithErrorMessage("A stay may be created at most one year in advance.");
    }

    [Fact]
    public void Check_in_exactly_one_year_ahead_is_allowed()
    {
        var checkIn = Today.AddYears(1);
        var request = ValidRequest() with { CheckIn = checkIn, CheckOut = checkIn.AddDays(2) };
        var result = CreateValidator().TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.CheckIn);
    }

    [Fact]
    public void Check_out_must_be_after_check_in()
    {
        var request = ValidRequest() with { CheckIn = Today.AddDays(5), CheckOut = Today.AddDays(5) };
        var result = CreateValidator().TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CheckOut)
            .WithErrorMessage("Check-out must be after check-in.");
    }

    [Fact]
    public void Stay_longer_than_one_month_is_rejected()
    {
        var checkIn = Today.AddDays(5);
        var request = ValidRequest() with { CheckIn = checkIn, CheckOut = checkIn.AddMonths(1).AddDays(1) };
        var result = CreateValidator().TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CheckOut)
            .WithErrorMessage("Maximum length of stay is one month.");
    }

    [Fact]
    public void Stay_of_exactly_one_month_is_allowed()
    {
        var checkIn = Today.AddDays(5);
        var request = ValidRequest() with { CheckIn = checkIn, CheckOut = checkIn.AddMonths(1) };
        var result = CreateValidator().TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.CheckOut);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Rooms_must_be_at_least_one(int rooms)
    {
        var request = ValidRequest() with { Rooms = rooms };
        var result = CreateValidator().TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Rooms);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Adults_must_be_at_least_one(int adults)
    {
        var request = ValidRequest() with { Adults = adults };
        var result = CreateValidator().TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Adults);
    }

    [Fact]
    public void Children_ages_within_range_pass()
    {
        var request = ValidRequest() with { ChildrenAges = [0, 5, 17] };
        var result = CreateValidator().TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.ChildrenAges);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(18)]
    public void Children_ages_outside_zero_to_seventeen_fail(int age)
    {
        var request = ValidRequest() with { ChildrenAges = [age] };
        var result = CreateValidator().TestValidate(request);
        result.ShouldHaveValidationErrorFor("ChildrenAges[0]");
    }

    [Fact]
    public void Null_children_ages_are_allowed()
    {
        var request = ValidRequest() with { ChildrenAges = null };
        var result = CreateValidator().TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Empty_hotel_id_is_rejected()
    {
        var request = ValidRequest() with { HotelId = Guid.Empty };
        var result = CreateValidator().TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.HotelId)
            .WithErrorMessage("Hotel identifier is required.");
    }
}
