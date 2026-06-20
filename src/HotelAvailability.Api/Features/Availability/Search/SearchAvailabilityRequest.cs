namespace HotelAvailability.Api.Features.Availability.Search;

/// <summary>
/// Request body for an availability search. A DTO distinct from the domain
/// <c>AvailabilityQuery</c>: it carries exactly what crosses the wire. <see cref="DateOnly"/>
/// is used for calendar dates and <see cref="ChildrenAges"/> is nullable/optional because
/// ages are only supplied when the booking includes children.
/// </summary>
public sealed record SearchAvailabilityRequest
{
    /// <summary>Hotel identifier to search.</summary>
    public Guid HotelId { get; init; }

    /// <summary>First night of the stay.</summary>
    public DateOnly CheckIn { get; init; }

    /// <summary>Departure day (exclusive).</summary>
    public DateOnly CheckOut { get; init; }

    /// <summary>Number of rooms requested.</summary>
    public int Rooms { get; init; }

    /// <summary>Number of adults.</summary>
    public int Adults { get; init; }

    /// <summary>Ages of children, when the booking includes children; otherwise null/omitted.</summary>
    public IReadOnlyList<int>? ChildrenAges { get; init; }
}
