using HotelAvailability.Api.Domain;

namespace HotelAvailability.Api.Data;

/// <summary>
/// Abstraction over the hotel reference-data source. Returns the full hotel list; pagination is a
/// slice concern. The backing store (in-memory mock, database, ...) is swappable behind this.
/// </summary>
public interface IHotelRepository
{
    ValueTask<IReadOnlyList<Hotel>> GetHotelsAsync(CancellationToken cancellationToken);
}
