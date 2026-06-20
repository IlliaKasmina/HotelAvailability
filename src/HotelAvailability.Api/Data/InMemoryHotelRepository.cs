using HotelAvailability.Api.Domain;

namespace HotelAvailability.Api.Data;

/// <summary>
/// Mock provider seeded with a fixed set of hotels (stable GUIDs). Stands in for a real data
/// source behind <see cref="IHotelRepository"/>. Returns the hotels ordered by name.
/// </summary>
public sealed class InMemoryHotelRepository : IHotelRepository
{
    private static readonly IReadOnlyList<Hotel> Hotels =
    [
        new(new HotelId(Guid.Parse("a1111111-1111-1111-1111-111111111111")), "Azure Bay Resort", "Nice", "France", 4),
        new(new HotelId(Guid.Parse("b2222222-2222-2222-2222-222222222222")), "Cedar Lodge", "Aspen", "United States", 3),
        new(new HotelId(Guid.Parse("c3333333-3333-3333-3333-333333333333")), "Dune View Inn", "Dubai", "United Arab Emirates", 4),
        new(new HotelId(Guid.Parse("d4444444-4444-4444-4444-444444444444")), "Emerald Garden Hotel", "Singapore", "Singapore", 5),
        new(new HotelId(Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479")), "Grand Plaza", "London", "United Kingdom", 5),
        new(new HotelId(Guid.Parse("e5555555-5555-5555-5555-555555555555")), "Harbour Lights", "Sydney", "Australia", 4),
        new(new HotelId(Guid.Parse("f6666666-6666-6666-6666-666666666666")), "Mountain Crest", "Zermatt", "Switzerland", 5),
        new(new HotelId(Guid.Parse("07777777-7777-7777-7777-777777777777")), "Riverside Suites", "Prague", "Czechia", 3),
    ];

    public ValueTask<IReadOnlyList<Hotel>> GetHotelsAsync(CancellationToken cancellationToken)
    {
        // Honour cancellation even on this synchronous mock path.
        cancellationToken.ThrowIfCancellationRequested();

        var ordered = Hotels.OrderBy(h => h.Name, StringComparer.Ordinal).ToList();
        return ValueTask.FromResult<IReadOnlyList<Hotel>>(ordered);
    }
}
