using HotelAvailability.Api.Common.ErrorHandling;
using HotelAvailability.Api.Data;
using HotelAvailability.Api.Domain;

namespace HotelAvailability.Api.Features.Availability.Search;

/// <summary>Search availability slice: endpoint wiring + handler.</summary>
public static class SearchAvailabilityEndpoint
{
    public static RouteGroupBuilder MapSearchAvailability(this RouteGroupBuilder group)
    {
        group.MapPost("/search", HandleAsync)
            .AddEndpointFilter<ValidationFilter<SearchAvailabilityRequest>>()
            .WithName("SearchAvailability")
            .WithSummary("Search available rooms in a hotel for a stay.")
            .Produces<AvailabilityResponse>()
            .ProducesValidationProblem();

        return group;
    }

    private static async Task<IResult> HandleAsync(
        SearchAvailabilityRequest request,
        IAvailabilityRepository repository,
        ILoggerFactory loggerFactory,
        CancellationToken cancellationToken)
    {
        var logger = loggerFactory.CreateLogger("HotelAvailability.Api.Features.Availability.Search");

        // Map the validated DTO to the domain query; children ages default to empty when omitted.
        var query = new AvailabilityQuery(
            new HotelId(request.HotelId),
            request.CheckIn,
            request.CheckOut,
            request.Rooms,
            request.Adults,
            request.ChildrenAges ?? []);

        logger.LogInformation(
            "Searching availability for hotel {HotelId} {CheckIn}->{CheckOut}: {Rooms} room(s), {Adults} adult(s), {Children} child(ren)",
            query.HotelId, query.CheckIn, query.CheckOut, query.Rooms, query.Adults, query.Children);

        var rooms = await repository.GetAvailableRoomsAsync(query, cancellationToken);

        logger.LogInformation(
            "Found {RoomCount} available room(s) for hotel {HotelId}",
            rooms.Count, query.HotelId);

        return TypedResults.Ok(rooms.ToResponse(query));
    }
}
