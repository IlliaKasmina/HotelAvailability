namespace HotelAvailability.Api.Domain;

/// <summary>A bookable hotel as reference data. Identified by a strongly-typed <see cref="HotelId"/>.</summary>
public sealed record Hotel(HotelId Id, string Name, string City, string Country, int StarRating);
