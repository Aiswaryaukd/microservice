# MyApp — Full Project Guide

This document explains the **model**, **how to run**, **which tables exist**, **how tables are created**, and **every step** from database insert to UI.

---

## 1. What this project is

MyApp is a .NET 10 learning solution:

- **Blazor UI** — screens the user sees
- **API Gateway** — one entry point for all `/api` calls
- **Product.Api** — REST APIs (Product, Message, Auth)
- **SQLite database** — file `Product.Api/app.db` with real tables

You must run **3 apps** together. The UI does not talk to the database directly.

```
Browser  →  BlazorApp (:5062)
              ↓ HTTP
         APIGateway (:5083)
              ↓ proxy
         Product.Api (:5033)
              ↓ EF Core
         SQLite app.db  (Products + Messages tables)
```

---

## 2. Understand the model (layers)

Each feature (Product, Message) uses the **same 4 layers**.

| Layer | Project | Job |
|-------|---------|-----|
| **Domain** | `Product.Domain` | Entity classes (table shape) and repository interfaces |
| **Application** | `Product.Application` | Business methods: GetAll, GetById, Create |
| **Infrastructure** | `Product.Infrastructure` | Database context, table mapping, insert/select |
| **API** | `Product.Api` | HTTP controllers + DTOs (JSON in/out) |
| **Gateway** | `APIGateway` | Forwards `/api/*` to Product.Api |
| **UI** | `BlazorApp` | Pages: list + create forms |

### Product model

| C# property | Database column | Type | Notes |
|-------------|-----------------|------|--------|
| `Id` | `Id` | INTEGER | Primary key, auto increment |
| `Name` | `Name` | TEXT | Required |
| `Price` | `Price` | DECIMAL | Price |

Entity file: `Product.Domain/Entities/Product.cs` (`ProductClass`)

### Message model

| C# property | Database column | Type | Notes |
|-------------|-----------------|------|--------|
| `Id` | `Id` | INTEGER | Primary key, auto increment |
| `Content` | `Content` | TEXT (max 500) | Required message text |

Entity file: `Product.Domain/Entities/Message.cs` (`MessageClass`)

---

## 3. Which section = which table

| Section in app | UI pages | API | Database table |
|----------------|----------|-----|----------------|
| **Products** | `/products`, `/products/create` | `/api/product` | **Products** |
| **Messages** | `/messages`, `/messages/create` | `/api/message` | **Messages** |
| **Auth (demo)** | none in UI yet | `/api/auth/login` | no table (hardcoded login) |

### Table: Products

| Column | Meaning |
|--------|---------|
| Id | Unique product number (created by database) |
| Name | Product name |
| Price | Product price |

### Table: Messages

| Column | Meaning |
|--------|---------|
| Id | Unique message number (created by database) |
| Content | Message text |

Database file location:

```
d:\Project\MyApp\MyApp\Product.Api\app.db
```

---

## 4. How the table is created (all steps)

You do **not** run SQL `CREATE TABLE` by hand. Entity Framework Core creates tables on API startup.

### Step A — Define entity (columns)

Example for Message:

```csharp
public class MessageClass
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
}
```

### Step B — Add DbSet (this is the table)

In `Product.Infrastructure/Data/ProductDbContext.cs`:

```csharp
public DbSet<ProductClass> Products { get; set; }
public DbSet<MessageClass> Messages { get; set; }
```

`ToTable("Messages")` and `ToTable("Products")` set the real table names.

### Step C — Choose SQLite connection

`Product.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=app.db"
}
```

`Product.Api/Program.cs`:

```csharp
options.UseSqlite(connectionString);
```

### Step D — Create tables at runtime

On API start:

```csharp
db.Database.EnsureCreated();
```

This:

1. Creates `app.db` if it does not exist
2. Creates table **Products**
3. Creates table **Messages**

### Step E — Insert seed rows (first run only)

If a table is empty, sample rows are inserted:

- Products: Sample A, Sample B, Sample C
- Messages: two welcome messages

Then `db.SaveChanges()` writes them to disk.

### Step F — Insert from UI / API

When you click **Save Message**:

1. UI POST `{ "content": "..." }` to `/api/message`
2. Controller maps DTO → `MessageClass`
3. Repository: `_context.Messages.AddAsync` + `SaveChangesAsync`
4. SQLite inserts a row and returns new **Id**
5. UI shows the list including the new row

Same pattern for Product create (`Name` + `Price`).

---

## 5. How to run (full steps)

### Prerequisites

1. Install **.NET 10 SDK**
2. Check: `dotnet --version` (should start with `10.`)
3. Open PowerShell in folder:

```powershell
cd d:\Project\MyApp\MyApp
```

### Ports

| App | Port | URL |
|-----|------|-----|
| Product.Api | 5033 | http://localhost:5033/swagger |
| APIGateway | 5083 | http://localhost:5083/api/product |
| BlazorApp | 5062 | http://localhost:5062 |

Use **http://**, not https://.

### Option 1 — One script (recommended)

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\start-all.ps1
```

Wait 30–60 seconds. Then open **http://localhost:5062**

The script:

1. Stops anything on ports 5033, 5083, 5062
2. Sets `ASPNETCORE_ENVIRONMENT=Development`
3. Starts Product.Api
4. Starts APIGateway
5. Starts BlazorApp

### Option 2 — Three terminals

**Terminal 1**

```powershell
dotnet run --project Product.Api\Product.Api.csproj --urls http://localhost:5033
```

**Terminal 2**

```powershell
dotnet run --project APIGateway\APIGateway.csproj --urls http://localhost:5083
```

**Terminal 3**

```powershell
dotnet run --project BlazorApp\BlazorApp.csproj --urls http://localhost:5062
```

If build fails with **file is locked**, stop running apps first, then run the script again.

### After start — what to click

| Step | Open | What you see |
|------|------|----------------|
| 1 | http://localhost:5062 | Home dashboard |
| 2 | **Products** | Table of products from **Products** table |
| 3 | **Create Product** | Form Name + Price → INSERT into Products |
| 4 | **Messages** | Table of messages from **Messages** table |
| 5 | **Create Message** | Form Content → INSERT into Messages |

---

## 6. API test commands

### Products

```powershell
# List
Invoke-RestMethod http://localhost:5033/api/product

# Insert
Invoke-RestMethod -Uri http://localhost:5033/api/product -Method Post -ContentType application/json -Body '{"name":"New Product","price":99.99}'
```

### Messages

```powershell
# List
Invoke-RestMethod http://localhost:5033/api/message

# Insert
Invoke-RestMethod -Uri http://localhost:5033/api/message -Method Post -ContentType application/json -Body '{"content":"My first message"}'
```

### Login (demo only)

```powershell
Invoke-RestMethod -Uri http://localhost:5083/api/auth/login -Method Post -ContentType application/json -Body '{"email":"admin@example.com","password":"Password123"}'
```

- Email: `admin@example.com`
- Password: `Password123`

Same URLs work through the gateway by changing `5033` → `5083`.

Swagger docs: **http://localhost:5033/swagger**

---

## 7. Request path (understand the flow)

### List messages

```
Browser /messages
  → Blazor Message.razor
  → GET http://localhost:5083/api/message
  → Gateway ocelot.json
  → GET http://localhost:5033/api/message
  → MessageController.Get
  → MessageService.GetAllAsync
  → MessageRepository (SELECT * FROM Messages)
  → JSON [{ id, content }]
  → Table in UI
```

### Create message (insert)

```
Browser /messages/create  (type content, click Save)
  → POST { content }
  → Gateway → MessageController.Create
  → new MessageClass { Content = ... }
  → INSERT INTO Messages (Content) VALUES (...)
  → database returns Id
  → redirect to /messages
```

---

## 8. Important files (by section)

### Database / tables

| File | Section |
|------|---------|
| `Product.Domain/Entities/Product.cs` | Product columns |
| `Product.Domain/Entities/Message.cs` | Message columns |
| `Product.Infrastructure/Data/ProductDbContext.cs` | Table mapping |
| `Product.Infrastructure/Repositories/ProductRepositories.cs` | Product SELECT/INSERT |
| `Product.Infrastructure/Repositories/MessageRepository.cs` | Message SELECT/INSERT |
| `Product.Api/Program.cs` | SQLite + EnsureCreated + seed |
| `Product.Api/appsettings.json` | Connection string |
| `Product.Api/app.db` | Real SQLite file (created at run) |

### APIs

| File | Section |
|------|---------|
| `Product.Api/Controllers/ProductController.cs` | Product HTTP |
| `Product.Api/Controllers/MessageController.cs` | Message HTTP |
| `Product.Api/Controllers/AuthController.cs` | Demo login |
| `APIGateway/ocelot.json` | Route map |

### UI

| File | Section |
|------|---------|
| `BlazorApp/Components/Pages/Product.razor` | Product list |
| `BlazorApp/Components/Pages/ProductCreate.razor` | Product create |
| `BlazorApp/Components/Pages/Message.razor` | Message list |
| `BlazorApp/Components/Pages/MessageCreate.razor` | Message create |
| `BlazorApp/UnitOfWork/productService.cs` | UI → Product API |
| `BlazorApp/UnitOfWork/messageService.cs` | UI → Message API |
| `BlazorApp/Program.cs` | HttpClient base URL `http://localhost:5083/` |

---

## 9. Add a new table / API (copy this checklist)

Use this whenever you add another feature (example name: `Order`).

1. Create entity in `Product.Domain/Entities` (`Id` + fields).
2. Create `IXxxRepository` in `Product.Domain/Abstract`.
3. Add `DbSet<Xxx> Xxxs` in `ProductDbContext` and `ToTable("Xxxs")`.
4. Create repository class: `AddAsync` + `SaveChangesAsync`.
5. Create `IXxxService` + `XxxService` in Application.
6. Register both in `Product.Api/Program.cs`.
7. Add DTOs in `Product.Api/Models`.
8. Add `XxxController` with GET list and POST create.
9. Add gateway route in `ocelot.json` (or use `/api/{everything}` fallback).
10. Add Blazor HttpClient service, list page, create page, nav links.
11. **Stop running apps**, rebuild, run `start-all.ps1`.
12. First start: `EnsureCreated()` creates the new table in `app.db`.

**Note:** If `app.db` already exists, `EnsureCreated()` will **not** add a new table to an old file. Delete `Product.Api/app.db` once (after stopping the API), then start again so tables are recreated. Product and Message seed data will be inserted again.

---

## 10. Common problems

| Problem | Cause | Fix |
|---------|--------|-----|
| Browser `ERR_CONNECTION_REFUSED` | Blazor not running | Run `start-all.ps1` |
| Products/Messages empty or error | API or Gateway down | Start all 3 services |
| Build "file locked" | Old `dotnet` still running | Stop processes, then rebuild |
| New table missing | Old `app.db` | Stop API, delete `app.db`, start again |
| Wrong URL | Using https | Use `http://localhost:5062` |

---

## 11. Quick daily workflow

1. `cd d:\Project\MyApp\MyApp`
2. `powershell -ExecutionPolicy Bypass -File .\scripts\start-all.ps1`
3. Open http://localhost:5062
4. Use **Products** and **Messages**
5. New rows are stored in `Product.Api/app.db`

**This project's files (Product/Message entity, controller, Blazor pages):** `Docs/CREATE-NEW-API.md`

More API notes: `Docs/API-SETUP.md`
