using HotelAvailability.Api.Domain;

namespace HotelAvailability.Api.Features.Hotels.List;

/// <summary>Builds the paged hotels response from the full domain list. Pure: no I/O.</summary>
internal static class ListHotelsMapping
{
    public static HotelsResponse BuildResponse(IReadOnlyList<Hotel> hotels, int page, int pageSize)
    {
        var totalCount = hotels.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = hotels
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto)
            .ToList();

        return new HotelsResponse(page, pageSize, totalCount, totalPages, items);
    }

    private static HotelDto ToDto(Hotel hotel) =>
        new(hotel.Id.Value, hotel.Name, hotel.City, hotel.Country, hotel.StarRating);
}
