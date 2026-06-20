namespace HotelAvailability.Api.Domain;

/// <summary>
/// A bookable rate for a room. Holds the total price for the whole stay, exactly one
/// cancellation policy, and optional board information (null when no meals are included).
/// </summary>
public sealed record RatePlan(
    RatePlanId Id,
    string Name,
    Money TotalPrice,
    CancellationPolicy Cancellation,
    Board? Board);
