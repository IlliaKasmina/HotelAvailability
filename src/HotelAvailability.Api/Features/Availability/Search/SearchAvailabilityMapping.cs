using HotelAvailability.Api.Domain;

namespace HotelAvailability.Api.Features.Availability.Search;

/// <summary>Maps domain models to response DTOs, keeping the wire shape decoupled from the domain.</summary>
internal static class SearchAvailabilityMapping
{
    public static AvailabilityResponse ToResponse(this IReadOnlyList<AvailableRoom> rooms, AvailabilityQuery query) =>
        new(
            query.HotelId.Value,
            query.CheckIn,
            query.CheckOut,
            query.Nights,
            [.. rooms.Select(ToDto)]);

    private static RoomDto ToDto(AvailableRoom room) =>
        new(room.Id.Value, room.Name, [.. room.RatePlans.Select(ToDto)]);

    private static RatePlanDto ToDto(RatePlan plan) =>
        new(
            plan.Id.Value,
            plan.Name,
            new MoneyDto(plan.TotalPrice.Amount, plan.TotalPrice.Currency),
            ToDto(plan.Cancellation),
            plan.Board is { } board ? new BoardDto(board.Type.ToString(), board.Description) : null);

    private static CancellationPolicyDto ToDto(CancellationPolicy policy) => policy switch
    {
        CancellationPolicy.NonRefundable => new NonRefundableDto(),
        CancellationPolicy.FreeCancellation free => new FreeCancellationDto(free.FreeUntil),
        _ => throw new ArgumentOutOfRangeException(nameof(policy), policy, "Unknown cancellation policy."),
    };
}
