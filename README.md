# Hotel Availability API

A small, vertical-slice REST API on **.NET 10** that searches hotel room availability and exposes a
(stubbed) booking endpoint. Built with Minimal APIs, FluentValidation, ProblemDetails error
handling, API-key authentication and Swagger.

## API Key

This is a test-assignment app, so the API key is shared here for convenience:

```
EWRDRQvP5dsn3dIRstgyMjPkIgtcqWFg
```

Pass it in the `X-Api-Key` header on every `/api/v1` request (it is the default configured in
`appsettings.json`). In a real application this would be a secret and would never be committed.

## Solution layout

```
HotelAvailability.slnx
src/
  HotelAvailability.Api             # the whole API: feature slices + shared concerns
    Features/
      Availability/Search/          # search slice: request, validator, response, mapping, endpoint
      Bookings/Create/              # create-booking slice (booking workflow is a 501 stub)
      Hotels/List/                  # paginated hotels-list slice
    Domain/                         # domain models (records, closed unions, strongly-typed IDs)
    Common/
      Authentication/               # API-key auth scheme
      ErrorHandling/                # ProblemDetails handlers + validation endpoint filter
    Data/                           # IAvailabilityRepository + IHotelRepository (in-memory providers)
tests/
  HotelAvailability.Tests           # xUnit v3 unit tests
```

The API is organised by **feature slice**: each endpoint owns its request, validation, response,
mapping and handler in one folder. Cross-cutting concerns shared by every slice live in `Common/`,
`Domain/` and `Data/`. `Program.cs` is the composition root that wires the slices together.

## Running

```bash
dotnet run --project src/HotelAvailability.Api
```

In `Development` the Swagger UI is served at `https://localhost:<port>/swagger`. Click
**Authorize** and paste the API key to try the secured endpoints from the browser.

## API key

All endpoints under `/api/v1` require an API key in the `X-Api-Key` header. The expected key
is read from configuration (`Authentication:ApiKey` in `appsettings.json`, default
`EWRDRQvP5dsn3dIRstgyMjPkIgtcqWFg`). Override it with an environment variable:

```bash
# configuration key "Authentication:ApiKey" -> env var "Authentication__ApiKey"
export Authentication__ApiKey="my-real-key"
```

A missing or wrong key returns **401** as ProblemDetails.

## Example requests

Base URL below assumes `http://localhost:5080`.

### 1. Valid search → 200

```bash
curl -X POST http://localhost:5080/api/v1/availability/search \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: EWRDRQvP5dsn3dIRstgyMjPkIgtcqWFg" \
  -d '{
        "hotelId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
        "checkIn": "2026-07-01",
        "checkOut": "2026-07-04",
        "rooms": 1,
        "adults": 2,
        "childrenAges": [5]
      }'
```

All identifiers are GUIDs. `hotelId` is supplied by the caller; `id` values for rooms and rate
plans are returned by the search and fed back into a booking request.

```json
{
  "hotelId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
  "checkIn": "2026-07-01",
  "checkOut": "2026-07-04",
  "nights": 3,
  "rooms": [
    {
      "id": "22222222-2222-2222-2222-222222222222",
      "name": "Deluxe King Room",
      "ratePlans": [
        {
          "id": "019ee0f0-52a0-7a96-a5b9-80b95b483e20",
          "name": "Flexible Rate",
          "totalPrice": { "amount": 483.00, "currency": "EUR" },
          "cancellation": { "type": "freeCancellation", "freeUntil": "2026-06-29T18:00:00+00:00" },
          "board": { "type": "BreakfastIncluded", "description": "Breakfast included" }
        },
        {
          "id": "019ee0f0-52a0-7612-8c39-d7b484fac8fc",
          "name": "Non-refundable Saver",
          "totalPrice": { "amount": 420, "currency": "EUR" },
          "cancellation": { "type": "nonRefundable" },
          "board": null
        }
      ]
    }
  ]
}
```

The `cancellation` object is a discriminated union: it is exactly one of
`{ "type": "nonRefundable" }` or `{ "type": "freeCancellation", "freeUntil": "..." }`.
`board` is omitted/`null` when the rate is room-only.

### 2. Invalid search (constraint violations) → 400

```bash
curl -X POST http://localhost:5080/api/v1/availability/search \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: EWRDRQvP5dsn3dIRstgyMjPkIgtcqWFg" \
  -d '{
        "hotelId": "",
        "checkIn": "2020-01-01",
        "checkOut": "2020-06-01",
        "rooms": 0,
        "adults": 0,
        "childrenAges": [20]
      }'
```

```json
{
  "type": "https://datatracker.ietf.org/doc/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "HotelId": ["Hotel identifier is required."],
    "Rooms": ["At least one room is required."],
    "Adults": ["At least one adult is required."],
    "CheckIn": ["A stay may start today at the earliest."],
    "CheckOut": ["Maximum length of stay is one month."],
    "ChildrenAges[0]": ["Each child's age must be between 0 and 17."]
  }
}
```

### 3. Create booking → 501 Not Implemented

```bash
curl -X POST http://localhost:5080/api/v1/bookings \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: EWRDRQvP5dsn3dIRstgyMjPkIgtcqWFg" \
  -d '{
        "hotelId": "f47ac10b-58cc-4372-a567-0e02b2c3d479",
        "roomId": "22222222-2222-2222-2222-222222222222",
        "ratePlanId": "019ee0f0-52a0-7a96-a5b9-80b95b483e20",
        "checkIn": "2026-07-01",
        "checkOut": "2026-07-04",
        "rooms": 1,
        "adults": 2,
        "lead": { "firstName": "Ada", "lastName": "Lovelace", "email": "ada@example.com" }
      }'
```

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.2",
  "title": "Not Implemented",
  "status": 501,
  "detail": "Booking creation is not yet implemented."
}
```

(A malformed booking body is rejected with a 400 ProblemDetails before reaching the 501 stub.)

### 4. List hotels (paginated) → 200

```bash
curl -X GET "http://localhost:5080/api/v1/hotels?page=1&pageSize=3" \
  -H "X-Api-Key: EWRDRQvP5dsn3dIRstgyMjPkIgtcqWFg"
```

```json
{
  "page": 1,
  "pageSize": 3,
  "totalCount": 8,
  "totalPages": 3,
  "items": [
    { "id": "a1111111-1111-1111-1111-111111111111", "name": "Azure Bay Resort", "city": "Nice", "country": "France", "starRating": 4 },
    { "id": "b2222222-2222-2222-2222-222222222222", "name": "Cedar Lodge", "city": "Aspen", "country": "United States", "starRating": 3 },
    { "id": "c3333333-3333-3333-3333-333333333333", "name": "Dune View Inn", "city": "Dubai", "country": "United Arab Emirates", "starRating": 4 }
  ]
}
```

Hotels are returned ordered by name. `page` defaults to 1 and `pageSize` to 20 (max 100); a page past
the end returns 200 with an empty `items` array. Invalid `page`/`pageSize` return a 400 ProblemDetails.

## Tests

```bash
dotnet test
```

Covers every validation rule (each date constraint, counts ≥ 1, children-age handling) and
the repository's room generation, pricing, occupancy filtering, and cancellation-token flow.
The hotels slice adds tests for its pagination validation, the seeded hotel repository, and the
pagination mapping (page boundaries, empty list). Uses xUnit v3 and FluentValidation's `TestHelper`.

## Where each graded feature lives

| Feature | Location |
|---|---|
| Precise types (`DateOnly`, `DateTimeOffset`, `decimal`) | `Domain/Money.cs`, `Domain/AvailabilityQuery.cs`, `Domain/RatePlan.cs`, slice DTOs |
| Nullable reference types, honest optionality (no `!`) | `Board?`, `ChildrenAges?`, `ApiKey?`; whole solution builds with 0 warnings |
| `record` / `record struct` | every domain model & DTO; `Money` is a `readonly record struct` |
| Strongly-typed IDs | `HotelId` / `RoomId` / `RatePlanId` in `Domain/Identifiers.cs`; DTOs stay primitive, converted in the slice |
| Closed/discriminated union | `Domain/CancellationPolicy.cs` → `CancellationPolicyDto` (polymorphic JSON) in the search slice |
| Dependency injection on abstractions | `IAvailabilityRepository`, `IHotelRepository`, `TimeProvider`, validators — registered in `Program.cs` |
| `CancellationToken` flow | endpoint handler → `IAvailabilityRepository` / `IHotelRepository` |
| Genuine async/await (no sync-over-async) | search handler, endpoints, `ValidationFilter` |
| Minimal API + route groups | `Program.cs`, `Features/**/...Endpoint.cs` |
| Vertical slices, DTO ≠ domain | `Features/` slices + `SearchAvailabilityMapping` |
| Paginated list endpoint | `Features/Hotels/List/` (slice) + `Data/InMemoryHotelRepository.cs` (seeded provider) |
| FluentValidation in the pipeline | `*Validator` + `Common/ErrorHandling/ValidationFilter<T>` |
| ProblemDetails (RFC 7807) | `Common/ErrorHandling/*Handler`, 401/403/501 |
| Swagger with API-key scheme | `AddSwaggerGen` security definition in `Program.cs` |
| API-key auth via auth handler | `Common/Authentication/ApiKeyAuthenticationHandler` |
| Structured logging | search & booking handlers, `ValidationFilter`, auth handler |
| Unit tests | `tests/HotelAvailability.Tests` |
