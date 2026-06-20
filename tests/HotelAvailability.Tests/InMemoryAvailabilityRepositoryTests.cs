using HotelAvailability.Api.Data;
using HotelAvailability.Api.Domain;

namespace HotelAvailability.Tests;

public sealed class InMemoryAvailabilityRepositoryTests
{
    private readonly InMemoryAvailabilityRepository _repository = new();

    private static AvailabilityQuery Query(int rooms = 1, int adults = 2, params int[] childrenAges) => new(
        HotelId: new HotelId(Guid.NewGuid()),
        CheckIn: new DateOnly(2026, 7, 1),
        CheckOut: new DateOnly(2026, 7, 4), // 3 nights
        Rooms: rooms,
        Adults: adults,
        ChildrenAges: childrenAges);

    [Fact]
    public async Task Each_room_exposes_a_refundable_and_a_non_refundable_rate()
    {
        var rooms = await _repository.GetAvailableRoomsAsync(Query(), TestContext.Current.CancellationToken);

        Assert.NotEmpty(rooms);
        foreach (var room in rooms)
        {
            Assert.Contains(room.RatePlans, r => r.Cancellation is CancellationPolicy.FreeCancellation);
            Assert.Contains(room.RatePlans, r => r.Cancellation is CancellationPolicy.NonRefundable);
        }
    }

    [Fact]
    public async Task Saver_rate_is_room_only_and_flexible_rate_includes_board()
    {
        var rooms = await _repository.GetAvailableRoomsAsync(Query(), TestContext.Current.CancellationToken);
        var room = rooms[0];

        var saver = Assert.Single(room.RatePlans, r => r.Cancellation is CancellationPolicy.NonRefundable);
        Assert.Null(saver.Board);

        var flexible = Assert.Single(room.RatePlans, r => r.Cancellation is CancellationPolicy.FreeCancellation);
        Assert.NotNull(flexible.Board);
        Assert.Equal(BoardType.BreakfastIncluded, flexible.Board.Type);
    }

    [Fact]
    public async Task Total_price_scales_with_nights_and_rooms()
    {
        var oneRoom = await _repository.GetAvailableRoomsAsync(Query(rooms: 1, adults: 2), TestContext.Current.CancellationToken);
        var twoRooms = await _repository.GetAvailableRoomsAsync(Query(rooms: 2, adults: 4), TestContext.Current.CancellationToken);

        var saverOne = oneRoom[0].RatePlans.Single(r => r.Cancellation is CancellationPolicy.NonRefundable);
        var saverTwo = twoRooms[0].RatePlans.Single(r => r.Cancellation is CancellationPolicy.NonRefundable);

        // Standard room: 95 * 3 nights = 285 for one room; doubled for two rooms.
        Assert.Equal(285m, saverOne.TotalPrice.Amount);
        Assert.Equal(570m, saverTwo.TotalPrice.Amount);
        Assert.Equal("EUR", saverOne.TotalPrice.Currency);
    }

    [Fact]
    public async Task Large_party_only_returns_rooms_that_fit()
    {
        // 5 guests in a single room: only the suite (max occupancy 5) qualifies.
        var rooms = await _repository.GetAvailableRoomsAsync(
            Query(rooms: 1, adults: 5), TestContext.Current.CancellationToken);

        var room = Assert.Single(rooms);
        Assert.Equal("Executive Suite", room.Name);
    }

    [Fact]
    public async Task Cancelled_token_throws_before_returning()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await _repository.GetAvailableRoomsAsync(Query(), cts.Token));
    }
}
