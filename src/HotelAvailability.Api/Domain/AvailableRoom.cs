namespace HotelAvailability.Api.Domain;

/// <summary>An available room with one or more bookable rate plans.</summary>
public sealed record AvailableRoom(
    RoomId Id,
    string Name,
    IReadOnlyList<RatePlan> RatePlans);
