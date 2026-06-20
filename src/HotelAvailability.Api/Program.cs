using FluentValidation;
using HotelAvailability.Api.Common.Authentication;
using HotelAvailability.Api.Common.ErrorHandling;
using HotelAvailability.Api.Data;
using HotelAvailability.Api.Features.Availability.Search;
using HotelAvailability.Api.Features.Bookings.Create;
using HotelAvailability.Api.Features.Hotels.List;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddProblemDetails();

// Mock providers are stateless and thread-safe, so singletons are appropriate.
builder.Services.AddSingleton<IAvailabilityRepository, InMemoryAvailabilityRepository>();
builder.Services.AddSingleton<IHotelRepository, InMemoryHotelRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddExceptionHandler<BadRequestExceptionHandler>();
builder.Services.AddExceptionHandler<UnhandledExceptionHandler>();

builder.Services
    .AddAuthentication(ApiKeyAuthenticationOptions.DefaultScheme)
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
        ApiKeyAuthenticationOptions.DefaultScheme,
        options => options.ApiKey = builder.Configuration["Authentication:ApiKey"]);
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Availability API",
        Version = "v1",
        Description = "Search hotel room availability. Protected by an API key.",
    });

    options.AddSecurityDefinition(ApiKeyAuthenticationOptions.DefaultScheme, new OpenApiSecurityScheme
    {
        Name = ApiKeyAuthenticationOptions.HeaderName,
        Type = SecuritySchemeType.ApiKey,
        In = ParameterLocation.Header,
        Description = $"API key supplied via the '{ApiKeyAuthenticationOptions.HeaderName}' header.",
    });

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(ApiKeyAuthenticationOptions.DefaultScheme, document)] = [],
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

// Swagger is served in every environment (incl. Release/Production) so the deployed
// app exposes its UI. This is a test-assignment app; a real one would gate this.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

// All endpoints live under /api/v1 and require a valid API key.
var api = app.MapGroup("/api/v1").RequireAuthorization();

api.MapGroup("/availability")
    .WithTags("Availability")
    .MapSearchAvailability();

api.MapGroup("/bookings")
    .WithTags("Bookings")
    .MapCreateBooking();

api.MapGroup("/hotels")
    .WithTags("Hotels")
    .MapListHotels();

app.Run();

