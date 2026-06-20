using HotelAvailability.Api.Common.ErrorHandling;

namespace HotelAvailability.Api.Features.Bookings.Create;

/// <summary>
/// Create-booking slice. The request is defined and validated, but the booking workflow is
/// intentionally not implemented: the handler returns 501 Not Implemented.
/// </summary>
public static class CreateBookingEndpoint
{
    public static RouteGroupBuilder MapCreateBooking(this RouteGroupBuilder group)
    {
        group.MapPost("/", HandleAsync)
            .AddEndpointFilter<ValidationFilter<CreateBookingRequest>>()
            .WithName("CreateBooking")
            .WithSummary("Create a booking from a chosen room and rate plan (not yet implemented).")
            .ProducesProblem(StatusCodes.Status501NotImplemented)
            .ProducesValidationProblem();

        return group;
    }

    private static IResult HandleAsync(CreateBookingRequest request, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger("HotelAvailability.Api.Features.Bookings.Create");
        logger.LogInformation(
            "Booking requested for hotel {HotelId}, room {RoomId}, rate {RatePlanId} - not implemented",
            request.HotelId, request.RoomId, request.RatePlanId);

        return TypedResults.Problem(
            title: "Not Implemented",
            detail: "Booking creation is not yet implemented.",
            statusCode: StatusCodes.Status501NotImplemented);
    }
}
