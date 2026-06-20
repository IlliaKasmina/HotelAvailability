using HotelAvailability.Api.Domain;

namespace HotelAvailability.Api.Data;

/// <summary>
/// Abstraction over the availability data source. The search slice depends on this interface,
/// not on any concrete provider, so the backing store (in-memory mock, database, external
/// supplier API, ...) is swappable without touching the feature handlers.
/// </summary>
public interface IAvailabilityRepository
{
    ValueTask<IReadOnlyList<AvailableRoom>> GetAvailableRoomsAsync(
        AvailabilityQuery query,
        CancellationToken cancellationToken);
}
