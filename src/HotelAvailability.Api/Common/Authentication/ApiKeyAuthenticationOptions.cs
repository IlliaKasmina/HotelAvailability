using Microsoft.AspNetCore.Authentication;

namespace HotelAvailability.Api.Common.Authentication;

/// <summary>Options for the API key authentication scheme. The key is bound from configuration.</summary>
public sealed class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public const string DefaultScheme = "ApiKey";
    public const string HeaderName = "X-Api-Key";

    /// <summary>The expected API key. Configured via <c>Authentication:ApiKey</c>.</summary>
    public string? ApiKey { get; set; }
}
