namespace HotelAvailability.Api.Domain;

/// <summary>
/// Closed (discriminated) hierarchy describing how a rate plan may be cancelled.
/// A rate plan holds exactly one of the nested cases, so it can never be both
/// refundable and non-refundable. The <c>private protected</c> constructor prevents
/// any type outside this file from extending the hierarchy, keeping the union closed.
/// </summary>
public abstract record CancellationPolicy
{
    private protected CancellationPolicy()
    {
    }

    /// <summary>The rate cannot be cancelled or refunded.</summary>
    public sealed record NonRefundable : CancellationPolicy;

    /// <summary>The rate can be cancelled free of charge until <paramref name="FreeUntil"/>.</summary>
    public sealed record FreeCancellation(DateTimeOffset FreeUntil) : CancellationPolicy;
}
