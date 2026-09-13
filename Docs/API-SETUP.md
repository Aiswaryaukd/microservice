API Setup and run instructions

1) Ensure .NET SDK is installed (matching project target, e.g. .NET 10).

2) Start services (recommended):

- From repository root (MyApp), run the provided PowerShell script:

  powershell -ExecutionPolicy Bypass -File .\scripts\start-all.ps1

  This script stops processes using the default ports (5033,5083,5062), then starts:
  - Product.Api on port 5033
  - APIGateway on port 5083 (proxies /api/* to Product.Api)
  - BlazorApp on port 5062

3) Manual steps (if you prefer):

- Build: `dotnet build MyApp.sln`
- Run Product.Api: `dotnet run --project Product.Api\Product.Api.csproj --urls http://localhost:5033`
- Run APIGateway: `dotnet run --project APIGateway\APIGateway.csproj --urls http://localhost:5083`
- Run BlazorApp: `dotnet run --project BlazorApp\BlazorApp.csproj --urls http://localhost:5062`

4) Demo auth credentials (demo AuthController):

- POST to `http://localhost:5083/api/auth/login` with JSON:

  {
    "email": "admin@example.com",
    "password": "Password123"
  }

  Response: 200 OK with a `token` field (`demo-token-abc123`) for demo/testing.

5) Notes:

- Ocelot config (`APIGateway/ocelot.json`) maps `/api/auth/*`, `/api/product/*`, and a fallback `/api/*` to Product.Api on port 5033.
- If ports change, update the script and `ocelot.json` ports accordingly.
- For production, replace the demo auth with a proper identity provider and secure tokens.
**Project Run & API Setup Guide**

This document explains how to run the solution locally, how the API is wired, and step-by-step guidance to add new API endpoints.

**Prerequisites**
- **.NET SDK**: Install .NET 10 SDK. Verify with `dotnet --version` (should start with `10.`).
- **Environment**: Windows (tested) — PowerShell commands used below.

**Ports used in this workspace**
- **API**: `http://localhost:5033`
- **Blazor UI**: `http://localhost:5062`

**Start the API and UI**
- Start Product API:
  ```powershell
  dotnet run --project d:\Project\MyApp\MyApp\Product.Api\Product.Api.csproj
  ```
- Start Blazor UI:
  ```powershell
  dotnet run --project d:\Project\MyApp\MyApp\BlazorApp\BlazorApp.csproj
  ```

**Verify endpoints**
- GET products:
  ```powershell
  Invoke-RestMethod http://localhost:5033/api/product
  ```
- Create product (POST):
  ```powershell
  Invoke-RestMethod -Uri http://localhost:5033/api/product -Method Post -ContentType application/json -Body '{ "name":"New product","price":12.5 }'
  ```
- Swagger UI (API docs): `http://localhost:5033/swagger/index.html`

**Common issues & quick fixes**
- Port already in use: find process and stop it:
  ```powershell
  Get-NetTCPConnection -LocalPort 5062 | Select LocalAddress,LocalPort,State,OwningProcess
  Stop-Process -Id <PID> -Force
  ```
- File locked during build (DLL in use): stop running app that holds the file (use `Stop-Process`).
- Runtime mismatch: retarget projects or install matching .NET SDK.

**How the API is structured (high level)**
- Domain: `Product.Domain` contains `ProductClass` (entity model).
- Infrastructure: `Product.Infrastructure` contains `ProductDbContext` (EF Core InMemory for local dev) and `ProductRepository`.
- Application: `Product.Application` contains `IProductService` and `ProductService` (business logic using repository).
- API: `Product.Api` exposes HTTP endpoints and maps DTOs ↔ domain models.
- Blazor UI: `BlazorApp` uses `ProductService` (HttpClient) to call `Product.Api`.

**How to add a new API endpoint (step-by-step)**
1. Define request/response DTOs in `Product.Api/Models` (e.g., `ProductCreateDto`, `ProductDto`).
2. Add validation attributes to DTO properties (e.g., `[Required]`, `[Range]`).
3. In `Product.Application` update `IProductService` if needed (add `GetByIdAsync`, `UpdateAsync`, etc.).
4. Implement repository methods in `Product.Infrastructure` to persist the data (DbContext + EF Core).
5. Register services in `Product.Api/Program.cs` (example):
   ```csharp
   builder.Services.AddDbContext<ProductDbContext>(options =>
       options.UseInMemoryDatabase("ProductDb"));
   builder.Services.AddScoped<IProductRepository, ProductRepository>();
   builder.Services.AddScoped<IProductService, ProductService>();
   ```
6. Create controller action in `Product.Api/Controllers`:
   - Accept DTOs (`[FromBody] ProductCreateDto model`).
   - Validate (`if (!ModelState.IsValid) return ValidationProblem(ModelState);`).
   - Map DTO -> domain model, call service, map result -> DTO, return appropriate status (e.g., `CreatedAtAction`).
7. Add Swagger if helpful (`builder.Services.AddSwaggerGen();` and `app.UseSwagger(); app.UseSwaggerUI();`).

**Switching to SQLite for persistence (optional)**
1. Add package to `Product.Infrastructure`/`Product.Api`: `Microsoft.EntityFrameworkCore.Sqlite` and `Microsoft.EntityFrameworkCore.Design`.
2. Change `AddDbContext` usage:
   ```csharp
   builder.Services.AddDbContext<ProductDbContext>(options =>
       options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
   ```
3. Add `dotnet-ef` tools and create migrations:
   ```powershell
   dotnet add d:\Project\MyApp\MyApp\Product.Infrastructure\Product.Infrastructure.csproj package Microsoft.EntityFrameworkCore.Design
   dotnet tool install --global dotnet-ef
   dotnet ef migrations add InitialCreate --project Product.Infrastructure --startup-project Product.Api
   dotnet ef database update --project Product.Infrastructure --startup-project Product.Api
   ```

**Testing & debugging tips**
- Use `Invoke-RestMethod` or `curl` to test API endpoints.
- Inspect logs in the terminal where apps run for exceptions and startup info.
- For CORS issues (browser calls), add CORS policy in `Program.cs` and apply it to controllers.

**Next improvements you can make**
- Return `ProblemDetails` and consistent validation errors from API.
- Add DTO mapping library (e.g., AutoMapper) for larger projects.
- Add unit/integration tests for controllers and services.

If you want, I can commit this file into the repo and add a short `README.md` link to it.
