namespace HotelAvailability.Api.Features.Bookings.Create;

/// <summary>
/// Request body to create a booking from a chosen room and rate plan. Only the request
/// type is defined here &mdash; the booking workflow itself is intentionally not implemented.
/// </summary>
public sealed record CreateBookingRequest
{
    public Guid HotelId { get; init; }

    /// <summary>Identifier of the chosen room (from a prior search).</summary>
    public Guid RoomId { get; init; }

    /// <summary>Identifier of the chosen rate plan (from a prior search).</summary>
    public Guid RatePlanId { get; init; }

    public DateOnly CheckIn { get; init; }

    public DateOnly CheckOut { get; init; }

    public int Rooms { get; init; }

    public int Adults { get; init; }

    public IReadOnlyList<int>? ChildrenAges { get; init; }

    /// <summary>Lead guest the booking is held under.</summary>
    public required GuestDetails Lead { get; init; }
}

public sealed record GuestDetails
{
    public required string FirstName { get; init; }

    public required string LastName { get; init; }

    public required string Email { get; init; }
}
