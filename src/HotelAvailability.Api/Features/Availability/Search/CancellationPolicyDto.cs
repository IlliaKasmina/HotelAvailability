using System.Text.Json.Serialization;

namespace HotelAvailability.Api.Features.Availability.Search;

/// <summary>
/// Transport shape of the cancellation policy. Mirrors the closed domain hierarchy as a
/// polymorphic DTO: System.Text.Json emits a <c>type</c> discriminator, so a serialized rate
/// plan is exactly one of the cases and can never be both refundable and non-refundable.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(NonRefundableDto), "nonRefundable")]
[JsonDerivedType(typeof(FreeCancellationDto), "freeCancellation")]
public abstract record CancellationPolicyDto;

public sealed record NonRefundableDto : CancellationPolicyDto;

public sealed record FreeCancellationDto(DateTimeOffset FreeUntil) : CancellationPolicyDto;
