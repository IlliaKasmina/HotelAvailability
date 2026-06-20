using FluentValidation;

namespace HotelAvailability.Api.Common.ErrorHandling;

/// <summary>
/// Endpoint filter that runs the FluentValidation validator for <typeparamref name="T"/>
/// before the handler. On failure it throws a <see cref="ValidationException"/>, which the
/// exception handler turns into an RFC 7807 ProblemDetails 400. The validator is resolved
/// from DI, so per-request dependencies (e.g. <see cref="TimeProvider"/>) flow in correctly.
/// </summary>
public sealed class ValidationFilter<T>(
    IValidator<T> validator,
    ILogger<ValidationFilter<T>> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
        {
            return await next(context);
        }

        var result = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
        if (!result.IsValid)
        {
            logger.LogWarning(
                "Validation failed for {RequestType}: {Errors}",
                typeof(T).Name,
                string.Join("; ", result.Errors.Select(e => e.ErrorMessage)));

            throw new ValidationException(result.Errors);
        }

        return await next(context);
    }
}
