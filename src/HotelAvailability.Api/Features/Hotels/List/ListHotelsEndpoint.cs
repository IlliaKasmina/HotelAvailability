using HotelAvailability.Api.Common.ErrorHandling;
using HotelAvailability.Api.Data;

namespace HotelAvailability.Api.Features.Hotels.List;

/// <summary>List-hotels slice: endpoint wiring + handler.</summary>
public static class ListHotelsEndpoint
{
    public static RouteGroupBuilder MapListHotels(this RouteGroupBuilder group)
    {
        group.MapGet("/", HandleAsync)
            .AddEndpointFilter<ValidationFilter<ListHotelsRequest>>()
            .WithName("ListHotels")
            .WithSummary("List hotels (paginated).")
            .Produces<HotelsResponse>()
            .ProducesValidationProblem();

        return group;
    }

    private static async Task<IResult> HandleAsync(
        [AsParameters] ListHotelsRequest request,
        IHotelRepository repository,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("HotelAvailability.Api.Features.Hotels.List");

        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 20;

        logger.LogInformation("Listing hotels: page {Page}, pageSize {PageSize}", page, pageSize);

        var hotels = await repository.GetHotelsAsync(cancellationToken);
        var response = ListHotelsMapping.BuildResponse(hotels, page, pageSize);

        logger.LogInformation("Returning {Count} of {Total} hotel(s)",
            response.Items.Count, response.TotalCount);

        return TypedResults.Ok(response);
    }
}
