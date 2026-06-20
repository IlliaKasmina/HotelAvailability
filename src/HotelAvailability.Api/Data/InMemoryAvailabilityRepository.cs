using HotelAvailability.Api.Domain;

namespace HotelAvailability.Api.Data;

/// <summary>
/// Mock provider that generates availability dynamically from the query. Stands in for a
/// real data source behind <see cref="IAvailabilityRepository"/>; swapping in a database or
/// supplier API is just another implementation of that interface.
/// </summary>
public sealed class InMemoryAvailabilityRepository : IAvailabilityRepository
{
    // Stable room IDs so a given room keeps the same identity across calls (reference data).
    private static readonly IReadOnlyList<RoomTemplate> Templates =
    [
        new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Standard Double Room", BaseNightlyRate: 95m, MaxOccupancy: 2),
        new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Deluxe King Room", BaseNightlyRate: 140m, MaxOccupancy: 3),
        new(Guid.Parse("33333333-3333-3333-3333-333333333333"), "Executive Suite", BaseNightlyRate: 220m, MaxOccupancy: 5),
    ];

    public ValueTask<IReadOnlyList<AvailableRoom>> GetAvailableRoomsAsync(
        AvailabilityQuery query,
        CancellationToken cancellationToken)
    {
        // Honour cancellation even on this synchronous mock path.
        cancellationToken.ThrowIfCancellationRequested();

        var guests = query.Adults + query.Children;
        var guestsPerRoom = (int)Math.Ceiling(guests / (double)query.Rooms);

        var rooms = Templates
            .Where(template => template.MaxOccupancy >= guestsPerRoom)
            .Select(template => BuildRoom(template, query))
            .ToList();

        return ValueTask.FromResult<IReadOnlyList<AvailableRoom>>(rooms);
    }

    private static AvailableRoom BuildRoom(RoomTemplate template, AvailabilityQuery query)
    {
        var nights = query.Nights;

        // Flexible rate: free cancellation until 18:00 UTC two days before arrival, breakfast included.
        var flexibleTotal = decimal.Round(template.BaseNightlyRate * 1.15m * nights * query.Rooms, 2);
        var freeUntil = new DateTimeOffset(
            query.CheckIn.AddDays(-2).ToDateTime(new TimeOnly(18, 0), DateTimeKind.Utc));

        var flexible = new RatePlan(
            Id: new RatePlanId(Guid.CreateVersion7()),
            Name: "Flexible Rate",
            TotalPrice: new Money(flexibleTotal, "EUR"),
            Cancellation: new CancellationPolicy.FreeCancellation(freeUntil),
            Board: new Board(BoardType.BreakfastIncluded, "Breakfast included"));

        // Saver rate: non-refundable, room only (no board information).
        var saverTotal = decimal.Round(template.BaseNightlyRate * nights * query.Rooms, 2);

        var saver = new RatePlan(
            Id: new RatePlanId(Guid.CreateVersion7()),
            Name: "Non-refundable Saver",
            TotalPrice: new Money(saverTotal, "EUR"),
            Cancellation: new CancellationPolicy.NonRefundable(),
            Board: null);

        return new AvailableRoom(new RoomId(template.Id), template.Name, [flexible, saver]);
    }

    private sealed record RoomTemplate(Guid Id, string Name, decimal BaseNightlyRate, int MaxOccupancy);
}
