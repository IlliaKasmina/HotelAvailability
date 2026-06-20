namespace HotelAvailability.Api.Domain;

/// <summary>
/// Strongly-typed identifiers. Each wraps a <see cref="Guid"/> in a <c>readonly record struct</c>
/// so the compiler rejects mixing them up (e.g. passing a <see cref="RoomId"/> where a
/// <see cref="HotelId"/> is expected). Zero-allocation and structural equality. These live in the
/// domain only.
/// </summary>
public readonly record struct HotelId(Guid Value);

public readonly record struct RoomId(Guid Value);

public readonly record struct RatePlanId(Guid Value);
