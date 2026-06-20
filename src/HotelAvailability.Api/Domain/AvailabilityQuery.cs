namespace HotelAvailability.Api.Domain;

/// <summary>
/// Domain representation of a validated availability search. This is the contract the
/// service and provider speak in &mdash; deliberately separate from the API request DTO.
/// Dates use <see cref="DateOnly"/> because a stay is expressed in calendar nights,
/// not instants in time.
/// </summary>
public sealed record AvailabilityQuery(
    HotelId HotelId,
    DateOnly CheckIn,
    DateOnly CheckOut,
    int Rooms,
    int Adults,
    IReadOnlyList<int> ChildrenAges)
{
    /// <summary>Number of nights in the stay.</summary>
    public int Nights => CheckOut.DayNumber - CheckIn.DayNumber;

    /// <summary>Number of children in the booking.</summary>
    public int Children => ChildrenAges.Count;
}
