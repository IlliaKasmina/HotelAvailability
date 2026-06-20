namespace HotelAvailability.Api.Domain;

/// <summary>The kind of meals included in a rate.</summary>
public enum BoardType
{
    RoomOnly,
    BreakfastIncluded,
    HalfBoard,
    FullBoard,
    AllInclusive,
}

/// <summary>
/// Meal / board information for a rate plan. Only present when meals are actually
/// included; a room-only rate carries no <see cref="Board"/> at all (the property is null).
/// </summary>
public sealed record Board(BoardType Type, string Description);
