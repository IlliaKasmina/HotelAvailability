namespace HotelAvailability.Api.Features.Hotels.List;

/// <summary>
/// Query parameters for the hotels list. Both are optional; the handler applies defaults
/// (page 1, page size 20). Nullable so "omitted" is distinct from an explicit 0.
/// </summary>
public sealed record ListHotelsRequest
{
    /// <summary>1-based page number.</summary>
    public int? Page { get; init; }

    /// <summary>Number of hotels per page (1-100).</summary>
    public int? PageSize { get; init; }
}
