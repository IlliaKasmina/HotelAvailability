using HotelAvailability.Api.Data;
using HotelAvailability.Api.Domain;

namespace HotelAvailability.Tests;

public sealed class InMemoryHotelRepositoryTests
{
    private readonly InMemoryHotelRepository _repository = new();

    [Fact]
    public async Task Returns_seeded_hotels()
    {
        var hotels = await _repository.GetHotelsAsync(TestContext.Current.CancellationToken);
        Assert.NotEmpty(hotels);
    }

    [Fact]
    public async Task Hotels_are_ordered_by_name()
    {
        var hotels = await _repository.GetHotelsAsync(TestContext.Current.CancellationToken);
        var names = hotels.Select(h => h.Name).ToList();
        var sorted = names.OrderBy(n => n, StringComparer.Ordinal).ToList();
        Assert.Equal(sorted, names);
    }

    [Fact]
    public async Task Includes_known_seed_hotel()
    {
        var hotels = await _repository.GetHotelsAsync(TestContext.Current.CancellationToken);
        Assert.Contains(hotels, h => h.Id == new HotelId(Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479")));
    }

    [Fact]
    public async Task Cancelled_token_throws_before_returning()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await _repository.GetHotelsAsync(cts.Token));
    }
}
