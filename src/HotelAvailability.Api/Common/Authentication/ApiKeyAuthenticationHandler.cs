using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace HotelAvailability.Api.Common.Authentication;

/// <summary>
/// Authenticates requests by comparing the <c>X-Api-Key</c> header against the configured key
/// using a fixed-time comparison. Failed challenges are written as 401 ProblemDetails.
/// </summary>
public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory loggerFactory,
    UrlEncoder encoder,
    IProblemDetailsService problemDetailsService)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, loggerFactory, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyAuthenticationOptions.HeaderName, out var providedValues)
            || providedValues.Count == 0
            || string.IsNullOrWhiteSpace(providedValues[0]))
        {
            // No credentials supplied: not authenticated, let the challenge run.
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var expectedKey = Options.ApiKey;
        if (string.IsNullOrEmpty(expectedKey))
        {
            Logger.LogError("API key authentication is enabled but no key is configured.");
            return Task.FromResult(AuthenticateResult.Fail("API key authentication is not configured."));
        }

        var providedKey = providedValues[0]!;
        var matches = CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(providedKey),
            Encoding.UTF8.GetBytes(expectedKey));

        if (!matches)
        {
            Logger.LogWarning("Rejected request with an invalid API key.");
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key."));
        }

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "api-client")],
            Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = Context,
            ProblemDetails = new ProblemDetails
            {
                Title = "Unauthorized",
                Status = StatusCodes.Status401Unauthorized,
                Detail = $"A valid '{ApiKeyAuthenticationOptions.HeaderName}' header is required.",
            },
        });
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = Context,
            ProblemDetails = new ProblemDetails
            {
                Title = "Forbidden",
                Status = StatusCodes.Status403Forbidden,
            },
        });
    }
}
