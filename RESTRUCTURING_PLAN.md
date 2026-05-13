# AirBnb Project — Clean Architecture Restructuring Plan

---

## Current State Analysis

### What exists today

```
AirBnb/
├── Controller/          5 controllers
├── Data/                AppDbContext
├── Dtos/                6 flat DTO files
├── Entities/            6 entity models
├── Interface/           3 interfaces (2 unused)
├── Services/            3 services (only PropertyService + infra services)
├── Migrations/
└── Program.cs
```

### Core problems

| Problem | Location | Impact |
|---|---|---|
| Business logic directly in controllers | BookingController, PaymentController, ReviewController | Untestable, violates SRP |
| Hardcoded HostId string | PropertyService.cs:38 | Broken feature, security issue |
| Direct `AppDbContext` injection into controllers | Booking, Payment, Review controllers | Bypasses abstraction, no mocking |
| `IAsyncRepository<T>` and `ISpecification<T>` defined but never used | Interface/ | Dead code, wasted design |
| No global error handling | Entire app | Inconsistent error responses |
| No DTO validation attributes | Dtos/ | Invalid data reaches business logic |
| No auth guard on `POST /addProperty` | PropertyController | Any user can create a property |
| `User.Identity.Name` used for user ID lookup | BookingController, ReviewController | Fragile, breaks if username changes |
| JWT secret and Azure connection string in appsettings.json plain text | appsettings.json | Security exposure |
| TokenService and AzureBlobStorageService registered as concrete classes (not interfaces) | Program.cs | Not mockable in tests |
| No Unit of Work — multiple `SaveChangesAsync` calls per request | PaymentController | Risk of partial data commits |

---

## Target Architecture

The project stays as a **single .NET project** (no multi-project solution needed at this scale), but folders become strict logical layers with a one-way dependency rule:

```
Controller → Service → Repository → Database
```

No layer skips another. No controller touches DbContext. No repository knows about HTTP.

### Proposed folder structure

```
AirBnb/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── PropertyController.cs
│   ├── BookingController.cs
│   ├── PaymentController.cs
│   └── ReviewController.cs
│
├── Core/
│   ├── Entities/               (Domain models — no framework dependencies)
│   │   ├── User.cs
│   │   ├── Property.cs
│   │   ├── Booking.cs
│   │   ├── Payment.cs
│   │   ├── Review.cs
│   │   └── PropertyImage.cs
│   │
│   ├── DTOs/                   (Organized by feature, not file per DTO)
│   │   ├── Auth/
│   │   │   ├── LoginDto.cs
│   │   │   ├── RegisterDto.cs
│   │   │   └── AuthResponseDto.cs
│   │   ├── Property/
│   │   │   ├── CreatePropertyDto.cs
│   │   │   ├── UpdatePropertyDto.cs
│   │   │   └── PropertyResponseDto.cs
│   │   ├── Booking/
│   │   │   ├── CreateBookingDto.cs
│   │   │   └── BookingResponseDto.cs
│   │   ├── Payment/
│   │   │   ├── CreatePaymentDto.cs
│   │   │   └── PaymentResponseDto.cs
│   │   └── Review/
│   │       ├── CreateReviewDto.cs
│   │       └── ReviewResponseDto.cs
│   │
│   ├── Interfaces/
│   │   ├── Repositories/
│   │   │   ├── IGenericRepository.cs       (replaces dead IAsyncRepository<T>)
│   │   │   ├── IPropertyRepository.cs
│   │   │   ├── IBookingRepository.cs
│   │   │   ├── IPaymentRepository.cs
│   │   │   └── IReviewRepository.cs
│   │   └── Services/
│   │       ├── IAuthService.cs
│   │       ├── IPropertyService.cs
│   │       ├── IBookingService.cs
│   │       ├── IPaymentService.cs
│   │       ├── IReviewService.cs
│   │       ├── ITokenService.cs
│   │       └── IBlobStorageService.cs
│   │
│   ├── Specifications/         (Specification pattern — actually wire it up)
│   │   ├── ISpecification.cs
│   │   ├── BaseSpecification.cs
│   │   ├── PropertiesWithImagesSpec.cs
│   │   └── BookingsByGuestSpec.cs
│   │
│   └── Common/
│       ├── ServiceResult.cs    (Result pattern — eliminates try/catch in controllers)
│       └── PagedResult.cs      (Pagination wrapper)
│
├── Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/     (EF Fluent API configs per entity)
│   │       ├── PropertyConfiguration.cs
│   │       ├── BookingConfiguration.cs
│   │       └── ReviewConfiguration.cs
│   ├── Repositories/
│   │   ├── GenericRepository.cs
│   │   ├── PropertyRepository.cs
│   │   ├── BookingRepository.cs
│   │   ├── PaymentRepository.cs
│   │   └── ReviewRepository.cs
│   └── ExternalServices/
│       ├── AzureBlobStorageService.cs
│       └── TokenService.cs
│
├── Services/                   (Application/business logic layer)
│   ├── AuthService.cs
│   ├── PropertyService.cs
│   ├── BookingService.cs
│   ├── PaymentService.cs
│   └── ReviewService.cs
│
├── Middleware/
│   └── GlobalExceptionMiddleware.cs
│
├── Migrations/
├── Program.cs
└── appsettings.json
```

---

## Design Patterns to Apply

### 1. Repository Pattern (fix what is already started)

`IAsyncRepository<T>` is defined but implemented by nothing. Replace it with a proper `IGenericRepository<T>` and implement it.

**Why:** Controllers currently inject `AppDbContext` directly. This makes unit testing impossible — you cannot mock EF Core's DbContext reliably. A repository gives you an interface to mock.

```csharp
// Core/Interfaces/Repositories/IGenericRepository.cs
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IReadOnlyList<T>> GetAllAsync();
    Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<int> CountAsync(ISpecification<T> spec);
}
```

Feature-specific repositories extend this:

```csharp
// Core/Interfaces/Repositories/IBookingRepository.cs
public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<bool> GuestHasBookedPropertyAsync(string guestId, int propertyId);
    Task<IReadOnlyList<Booking>> GetBookingsByGuestAsync(string guestId);
}
```

**SOLID connection — D (Dependency Inversion):** Services depend on `IBookingRepository` (abstraction), not `AppDbContext` (concretion).

---

### 2. Service Layer Pattern

Every controller currently that touches `AppDbContext` gets its own service. The service is the only place where business rules live.

**Why:** Business rules do not belong in controllers. Controllers translate HTTP → service call → HTTP response. Services decide *what* to do. Repositories decide *how* to persist it.

```csharp
// Services/BookingService.cs
public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IPropertyRepository _propertyRepo;

    public BookingService(IBookingRepository bookingRepo, IPropertyRepository propertyRepo)
    {
        _bookingRepo = bookingRepo;
        _propertyRepo = propertyRepo;
    }

    public async Task<ServiceResult<BookingResponseDto>> CreateBookingAsync(
        CreateBookingDto dto, string guestId)
    {
        var property = await _propertyRepo.GetByIdAsync(dto.PropertyId);
        if (property is null)
            return ServiceResult<BookingResponseDto>.Fail("Property not found");

        var days = (dto.EndDate - dto.StartDate).Days;
        if (days <= 0)
            return ServiceResult<BookingResponseDto>.Fail("End date must be after start date");

        var booking = new Booking
        {
            PropertyId = dto.PropertyId,
            GuestId = guestId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            TotalAmount = days * property.PricePerNight,
            IsPaid = false
        };

        await _bookingRepo.AddAsync(booking);
        return ServiceResult<BookingResponseDto>.Ok(booking.ToResponseDto());
    }
}
```

**SOLID connection — S (Single Responsibility):** Each service handles one domain feature only.

---

### 3. Result Pattern (ServiceResult<T>)

Controllers currently return raw data or throw exceptions. Instead, every service method returns a `ServiceResult<T>` — a discriminated union of success or failure.

**Why:** No exceptions for expected business failures (property not found, guest hasn't booked). The controller checks the result and maps it to the correct HTTP status code.

```csharp
// Core/Common/ServiceResult.cs
public class ServiceResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public int StatusCode { get; private set; }

    public static ServiceResult<T> Ok(T data) =>
        new() { IsSuccess = true, Data = data, StatusCode = 200 };

    public static ServiceResult<T> Fail(string error, int statusCode = 400) =>
        new() { IsSuccess = false, Error = error, StatusCode = statusCode };

    public static ServiceResult<T> NotFound(string error) =>
        Fail(error, 404);

    public static ServiceResult<T> Forbidden(string error) =>
        Fail(error, 403);
}
```

Controller becomes:

```csharp
[HttpPost]
public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
{
    var guestId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var result = await _bookingService.CreateBookingAsync(dto, guestId!);

    return result.IsSuccess
        ? Ok(result.Data)
        : StatusCode(result.StatusCode, new { error = result.Error });
}
```

**SOLID connection — O (Open/Closed):** New result types (e.g., `Conflict`, `Unauthorized`) extend without touching existing code.

---

### 4. Specification Pattern (fix what is already defined)

`ISpecification<T>` is defined but never used. It should drive all filtered queries instead of writing raw LINQ in repositories.

**Why:** Without specs, every filter combination requires a new repository method. Specs make query logic composable and testable in isolation.

```csharp
// Core/Specifications/BaseSpecification.cs
public class BaseSpecification<T> : ISpecification<T>
{
    public Expression<Func<T, bool>>? Criteria { get; private set; }
    public List<Expression<Func<T, object>>> Includes { get; } = new();
    public Expression<Func<T, object>>? OrderBy { get; private set; }
    public int Take { get; private set; }
    public int Skip { get; private set; }
    public bool IsPagingEnabled { get; private set; }

    protected void AddCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;
    protected void AddInclude(Expression<Func<T, object>> include) => Includes.Add(include);
    protected void ApplyPaging(int skip, int take) { Skip = skip; Take = take; IsPagingEnabled = true; }
    protected void ApplyOrderBy(Expression<Func<T, object>> orderBy) => OrderBy = orderBy;
}

// Core/Specifications/PropertiesWithImagesSpec.cs
public class PropertiesWithImagesSpec : BaseSpecification<Property>
{
    public PropertiesWithImagesSpec()
    {
        AddInclude(p => p.Host);
    }

    public PropertiesWithImagesSpec(int id) : this()
    {
        AddCriteria(p => p.Id == id);
    }
}
```

**SOLID connection — O (Open/Closed):** New query filters = new spec class. No changes to the repository.

---

### 5. Unit of Work Pattern

PaymentController currently calls `SaveChangesAsync` twice — once when it saves the Payment, and once (implicit) when it updates the Booking. If the second save fails, the data is inconsistent.

**Why:** Unit of Work wraps multiple repository operations in a single database transaction, committed or rolled back together.

```csharp
// Core/Interfaces/IUnitOfWork.cs
public interface IUnitOfWork : IDisposable
{
    IPropertyRepository Properties { get; }
    IBookingRepository Bookings { get; }
    IPaymentRepository Payments { get; }
    IReviewRepository Reviews { get; }
    Task<int> CompleteAsync();
}
```

Usage in PaymentService:

```csharp
public async Task<ServiceResult<PaymentResponseDto>> ProcessPaymentAsync(
    CreatePaymentDto dto, string guestId)
{
    var booking = await _uow.Bookings.GetByIdAsync(dto.BookingId);
    if (booking is null) return ServiceResult<PaymentResponseDto>.NotFound("Booking not found");
    if (booking.GuestId != guestId) return ServiceResult<PaymentResponseDto>.Forbidden("Not your booking");

    var payment = new Payment { BookingId = dto.BookingId, Amount = booking.TotalAmount,
        PaymentStatus = "Completed", PaymentDate = DateTime.UtcNow };

    booking.IsPaid = true;

    await _uow.Payments.AddAsync(payment);
    await _uow.Bookings.UpdateAsync(booking);
    await _uow.CompleteAsync();   // single transaction — both or neither

    return ServiceResult<PaymentResponseDto>.Ok(payment.ToResponseDto());
}
```

---

### 6. Middleware — Global Exception Handler

All `try/catch` blocks currently scattered across controllers (there are none at present, which is worse) get replaced by a single middleware.

```csharp
// Middleware/GlobalExceptionMiddleware.cs
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new { error = "An unexpected error occurred." });
        }
    }
}
```

Register in Program.cs before everything else:
```csharp
app.UseMiddleware<GlobalExceptionMiddleware>();
```

---

## Interface Segregation — Split IPropertyInterface

The current `IPropertyInterface` does too much. Under ISP (Interface Segregation Principle), a controller that only reads properties should not be forced to depend on an interface that also declares write/upload operations.

**Current (violates ISP):**
```csharp
public interface IPropertyInterface
{
    Task AddProperty(...);
    Task<IEnumerable<Property>> GetAllProperties();
    Task<Property> GetPropertiesById(int id);
    Task UpdateProperty(...);
    Task DeleteProperty(...);
    Task UploadPropertyImage(...);
}
```

**After refactoring:**
```csharp
// Core/Interfaces/Services/IPropertyService.cs
public interface IPropertyReadService
{
    Task<ServiceResult<IReadOnlyList<PropertyResponseDto>>> GetAllAsync();
    Task<ServiceResult<PropertyResponseDto>> GetByIdAsync(int id);
}

public interface IPropertyWriteService
{
    Task<ServiceResult<PropertyResponseDto>> CreateAsync(CreatePropertyDto dto, string hostId);
    Task<ServiceResult<PropertyResponseDto>> UpdateAsync(int id, UpdatePropertyDto dto, string hostId);
    Task<ServiceResult<bool>> DeleteAsync(int id, string hostId);
}

// IPropertyService combines both (used by the controller that needs all operations)
public interface IPropertyService : IPropertyReadService, IPropertyWriteService { }
```

---

## DTO Validation — Add Data Annotations

Currently no validation. Add `[Required]`, `[Range]`, `[StringLength]` to all DTOs so invalid requests fail at model binding, before they reach service logic.

```csharp
// Core/DTOs/Booking/CreateBookingDto.cs
public class CreateBookingDto
{
    [Required]
    public int PropertyId { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    [DateGreaterThan("StartDate", ErrorMessage = "End date must be after start date")]
    public DateTime EndDate { get; set; }
}

// Core/DTOs/Review/CreateReviewDto.cs
public class CreateReviewDto
{
    [Required]
    public int PropertyId { get; set; }

    [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
    public int Rating { get; set; }

    [StringLength(1000)]
    public string Comment { get; set; } = string.Empty;
}
```

---

## Fix: User ID from Claims

Both `BookingController` and `ReviewController` call `User.Identity.Name` which returns the **username (email)**, not the user ID. This is fragile and will break authorization checks.

**Wrong (current):**
```csharp
GuestId = User.Identity.Name  // returns email, not the GUID user ID
```

**Correct (after fix):**
```csharp
var guestId = User.FindFirstValue(ClaimTypes.NameIdentifier); // returns the user's Id claim
```

This requires verifying that `TokenService` sets `ClaimTypes.NameIdentifier` to `user.Id` — it already does, so only the controllers need updating.

---

## Fix: Remove Hardcoded HostId

`PropertyService.cs:38` contains:
```csharp
HostId = "15383F0B-069C-48E5-80B7-82FEEF01E50D"
```

After adding `[Authorize(Roles = "Host")]` to `POST /addProperty`, the controller passes the authenticated user's ID to the service:

```csharp
// Controller
[HttpPost]
[Authorize(Roles = "Host")]
public async Task<IActionResult> AddProperty([FromForm] CreatePropertyDto dto)
{
    var hostId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var result = await _propertyService.CreateAsync(dto, hostId!);
    ...
}

// Service — no longer hardcodes anything
public async Task<ServiceResult<PropertyResponseDto>> CreateAsync(CreatePropertyDto dto, string hostId)
{
    var imageUrl = await _blobService.UploadImageAsync(dto.ImageFile.OpenReadStream(), dto.ImageFile.FileName);
    var property = new Property { ..., HostId = hostId };
    ...
}
```

---

## Secrets Management

JWT secret key and Azure connection string must not be in plain `appsettings.json`.

**Recommended approach for development:**
```bash
dotnet user-secrets set "JwtSettings:SecretKey" "your-actual-secret"
dotnet user-secrets set "AzureStorage:ConnectionString" "your-azure-conn"
```

**For production:** Use Azure Key Vault or environment variables injected by the hosting environment. Never commit secrets to source control.

---

## Response DTOs — Stop Returning Entities

Controllers currently return raw `Property`, `Booking`, etc. entities directly. This exposes internal model structure, makes API versioning painful, and serializes navigation properties causing circular reference issues.

Every service method returns a **response DTO**:

```csharp
// Core/DTOs/Property/PropertyResponseDto.cs
public class PropertyResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
}
```

Map inside the service (manual mapping or AutoMapper — either works, but AutoMapper adds a dependency for marginal benefit at this scale; manual mapping via extension methods is cleaner):

```csharp
// Infrastructure/Extensions/PropertyMappingExtensions.cs
public static class PropertyMappingExtensions
{
    public static PropertyResponseDto ToResponseDto(this Property p) => new()
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        PricePerNight = p.PricePerNight,
        ImageUrl = p.ImageUrl,
        HostName = p.Host?.FullName ?? string.Empty
    };
}
```

---

## Updated Program.cs Registration

```csharp
// Repositories
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IPropertyRepository, PropertyRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPropertyService, PropertyService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

// Infrastructure services (registered against interfaces now — mockable)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
```

---

## SOLID Principles — Where Each One Is Applied

| Principle | Violation Today | Fix Applied |
|---|---|---|
| **S** — Single Responsibility | Controllers contain database calls, business rules, and HTTP response logic all in one | Controllers: HTTP only. Services: business rules only. Repositories: data access only. |
| **O** — Open/Closed | Adding a new filter requires editing existing repository methods | Specification pattern — new filter = new spec class, no existing code touched |
| **L** — Liskov Substitution | `IPropertyInterface` methods return raw entities; a substitute could return DTOs and break callers | Service interfaces return `ServiceResult<T>` consistently — any implementation is substitutable |
| **I** — Interface Segregation | `IPropertyInterface` forces all callers to depend on CRUD + upload methods | Split into `IPropertyReadService` / `IPropertyWriteService` |
| **D** — Dependency Inversion | Controllers inject `AppDbContext` directly (concretion) | Controllers depend on `IBookingService` (abstraction); services depend on `IBookingRepository` (abstraction) |

---

## What This Enables for Testing

After restructuring, each layer is independently testable:

**Unit test a service** (no database, no HTTP):
```csharp
[Fact]
public async Task CreateBooking_WhenEndDateBeforeStartDate_ReturnsFailure()
{
    var mockBookingRepo = new Mock<IBookingRepository>();
    var mockPropertyRepo = new Mock<IPropertyRepository>();
    mockPropertyRepo.Setup(r => r.GetByIdAsync(1))
        .ReturnsAsync(new Property { PricePerNight = 100 });

    var service = new BookingService(mockBookingRepo.Object, mockPropertyRepo.Object);
    var dto = new CreateBookingDto { PropertyId = 1,
        StartDate = DateTime.Today.AddDays(5), EndDate = DateTime.Today };

    var result = await service.CreateBookingAsync(dto, "guest-id");

    Assert.False(result.IsSuccess);
    Assert.Equal(400, result.StatusCode);
}
```

**Integration test a controller** (real DI, mocked service):
```csharp
// Replace service implementation in test DI setup
services.AddScoped<IBookingService>(_ => mockBookingService.Object);
```

---

## Implementation Order

Do these in sequence — each step builds on the last and keeps the app compilable between steps.

1. **Create folder skeleton** — add all empty directories
2. **Move entities** to `Core/Entities/` — no logic changes
3. **Create `ServiceResult<T>`** — needed by everything else
4. **Create all interfaces** — `IGenericRepository<T>`, feature repos, service interfaces
5. **Implement `GenericRepository<T>`** and `UnitOfWork`
6. **Implement feature repositories** — `PropertyRepository`, `BookingRepository`, `PaymentRepository`, `ReviewRepository`
7. **Create `BookingService`** — move logic from `BookingController`
8. **Create `PaymentService`** — move logic from `PaymentController`
9. **Create `ReviewService`** — move logic from `ReviewController`
10. **Refactor `PropertyService`** — remove hardcoded HostId, wire up `IPropertyRepository`
11. **Create `AuthService`** — extract logic from `AuthController`
12. **Add `GlobalExceptionMiddleware`** — register in Program.cs
13. **Add DTO validation attributes** — all DTO files
14. **Create response DTOs** and mapping extension methods
15. **Slim down all controllers** — inject service interfaces, remove DbContext
16. **Update Program.cs** — register new interfaces and implementations
17. **Move secrets to user-secrets** — remove from appsettings.json
18. **Add missing endpoints** — `GET /reviews/{propertyId}`

---

## Summary of What Changes vs. What Stays

| Component | Action | Reason |
|---|---|---|
| `Entities/` | Move to `Core/Entities/` | Organization only |
| `Dtos/` | Reorganize into `Core/DTOs/{Feature}/` + add response DTOs | Feature grouping + avoid leaking entities |
| `IAsyncRepository<T>` | Replace with `IGenericRepository<T>` | Dead code → working abstraction |
| `ISpecification<T>` / `IPropertyInterface` | Keep + fix | Already correct design, not wired up |
| `AppDbContext` | Move to `Infrastructure/Data/`, add entity configs | Separate EF concerns |
| `PropertyService` | Refactor in place | Remove hardcoded HostId, use repository |
| `TokenService` | Move to `Infrastructure/ExternalServices/`, extract `ITokenService` | Make it mockable |
| `AzureBlobStorageService` | Move to `Infrastructure/ExternalServices/`, extract `IBlobStorageService` | Make it mockable |
| `BookingController` | Slim to HTTP-only, inject `IBookingService` | Remove business logic |
| `PaymentController` | Slim to HTTP-only, inject `IPaymentService` | Remove business logic, fix transaction |
| `ReviewController` | Slim to HTTP-only, inject `IReviewService` | Remove business logic |
| `AuthController` | Slim to HTTP-only, inject `IAuthService` | Separation of concerns |
| `Program.cs` | Update registrations | New interfaces |
| `appsettings.json` | Remove secrets | Move to user-secrets / Key Vault |
