namespace HotelAvailability.Api.Features.Availability.Search;

/// <summary>Response DTO for an availability search.</summary>
public sealed record AvailabilityResponse(
    Guid HotelId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Nights,
    IReadOnlyList<RoomDto> Rooms);

public sealed record RoomDto(
    Guid Id,
    string Name,
    IReadOnlyList<RatePlanDto> RatePlans);

public sealed record RatePlanDto(
    Guid Id,
    string Name,
    MoneyDto TotalPrice,
    CancellationPolicyDto Cancellation,
    BoardDto? Board);

public sealed record MoneyDto(decimal Amount, string Currency);

public sealed record BoardDto(string Type, string Description);
