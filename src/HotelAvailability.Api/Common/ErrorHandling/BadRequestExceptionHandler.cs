using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelAvailability.Api.Common.ErrorHandling;

/// <summary>
/// Maps a <see cref="BadHttpRequestException"/> (e.g. a body that fails to deserialize, such as
/// a malformed GUID) to a ProblemDetails using the exception's own status code (400), instead of
/// letting it fall through to the 500 fallback handler.
/// </summary>
public sealed class BadRequestExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badRequest)
        {
            return false;
        }

        httpContext.Response.StatusCode = badRequest.StatusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Title = "The request could not be processed.",
                Status = badRequest.StatusCode,
                Detail = "The request body is malformed or contains values of the wrong type.",
            },
            Exception = exception,
        });
    }
}
