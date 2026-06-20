namespace HotelAvailability.Api.Domain;

/// <summary>
/// A monetary amount in a given currency. Modeled as a <c>readonly record struct</c>:
/// money is a small, immutable value with structural equality, and <see cref="decimal"/>
/// (never <c>double</c>) is the correct type for currency arithmetic.
/// </summary>
public readonly record struct Money(decimal Amount, string Currency)
{
    public override string ToString() => $"{Amount:0.00} {Currency}";
}
