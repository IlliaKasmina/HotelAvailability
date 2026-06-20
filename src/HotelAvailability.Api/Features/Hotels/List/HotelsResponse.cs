namespace HotelAvailability.Api.Features.Hotels.List;

/// <summary>A page of hotels.</summary>
public sealed record HotelsResponse(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    IReadOnlyList<HotelDto> Items);

public sealed record HotelDto(Guid Id, string Name, string City, string Country, int StarRating);
