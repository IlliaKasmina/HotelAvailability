using HotelAvailability.Api.Domain;
using HotelAvailability.Api.Features.Hotels.List;

namespace HotelAvailability.Tests;

public sealed class ListHotelsMappingTests
{
    private static IReadOnlyList<Hotel> SampleHotels(int count) =>
        Enumerable.Range(1, count)
            .Select(i => new Hotel(new HotelId(Guid.NewGuid()), $"Hotel {i:D2}", "City", "Country", 3))
            .ToList();

    [Fact]
    public void First_page_returns_first_page_size_items()
    {
        var hotels = SampleHotels(25);

        var response = ListHotelsMapping.BuildResponse(hotels, page: 1, pageSize: 10);

        Assert.Equal(1, response.Page);
        Assert.Equal(10, response.PageSize);
        Assert.Equal(25, response.TotalCount);
        Assert.Equal(3, response.TotalPages);
        Assert.Equal(10, response.Items.Count);
        Assert.Equal("Hotel 01", response.Items[0].Name);
        Assert.Equal("Hotel 10", response.Items[9].Name);
    }

    [Fact]
    public void Last_partial_page_returns_remainder()
    {
        var hotels = SampleHotels(25);

        var response = ListHotelsMapping.BuildResponse(hotels, page: 3, pageSize: 10);

        Assert.Equal(5, response.Items.Count);
        Assert.Equal("Hotel 21", response.Items[0].Name);
    }

    [Fact]
    public void Page_past_end_returns_empty_items_with_correct_totals()
    {
        var hotels = SampleHotels(5);

        var response = ListHotelsMapping.BuildResponse(hotels, page: 99, pageSize: 10);

        Assert.Empty(response.Items);
        Assert.Equal(5, response.TotalCount);
        Assert.Equal(1, response.TotalPages);
    }

    [Fact]
    public void Empty_list_has_zero_total_pages()
    {
        var response = ListHotelsMapping.BuildResponse([], page: 1, pageSize: 10);

        Assert.Empty(response.Items);
        Assert.Equal(0, response.TotalCount);
        Assert.Equal(0, response.TotalPages);
    }

    [Fact]
    public void Hotel_fields_are_mapped()
    {
        var id = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");
        var hotels = new List<Hotel> { new(new HotelId(id), "Grand Plaza", "London", "United Kingdom", 5) };

        var response = ListHotelsMapping.BuildResponse(hotels, page: 1, pageSize: 10);

        var dto = Assert.Single(response.Items);
        Assert.Equal(id, dto.Id);
        Assert.Equal("Grand Plaza", dto.Name);
        Assert.Equal("London", dto.City);
        Assert.Equal("United Kingdom", dto.Country);
        Assert.Equal(5, dto.StarRating);
    }
}
