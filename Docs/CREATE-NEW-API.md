# This project — which file to create (Product + Message)

This document is **only for MyApp**. It lists the **real files** in this repo.

Work in two parts:

1. **Backend first:** Entity → table → API controller  
2. **After that model is complete:** Blazor App files  

Do not create Blazor pages before the API works.

---

## Part A — Product section (already in this project)

| What | File in this project |
|------|----------------------|
| Entity | `Product.Domain/Entities/Product.cs` (`ProductClass`: Id, Name, Price) |
| Repository interface | `Product.Domain/Abstract/IProductRepository.cs` |
| Table mapping | `Product.Infrastructure/Data/ProductDbContext.cs` → `DbSet` **Products** |
| Insert/select | `Product.Infrastructure/Repositories/ProductRepositories.cs` |
| Service interface | `Product.Application/Abstract/IProductService.cs` |
| Service | `Product.Application/Default/ProductService.cs` |
| API DTO list | `Product.Api/Models/ProductDto.cs` |
| API DTO create | `Product.Api/Models/ProductCreateDto.cs` |
| **API controller** | `Product.Api/Controllers/ProductController.cs` |
| Register | `Product.Api/Program.cs` (`IProductRepository`, `IProductService`) |

**After Product API is complete, Blazor files:**

| What | File in this project |
|------|----------------------|
| HTTP client | `BlazorApp/UnitOfWork/productService.cs` |
| Register HttpClient | `BlazorApp/Program.cs` (`ProductService` → `http://localhost:5083/`) |
| List page | `BlazorApp/Components/Pages/Product.razor` |
| List code | `BlazorApp/Components/Pages/Product.razor.cs` |
| List style | `BlazorApp/Components/Pages/Product.razor.css` |
| Create page | `BlazorApp/Components/Pages/ProductCreate.razor` |
| Create style | `BlazorApp/Components/Pages/ProductCreate.razor.css` |
| Menu links | `BlazorApp/Components/Layout/NavMenu.razor` (`/products`, `/products/create`) |

**UI URLs:** `/products` and `/products/create`  
**API:** `GET/POST http://localhost:5033/api/product`

---

## Part B — Message section (already in this project)

Create files in this **same order**.

### 1. Entity

File: `Product.Domain/Entities/Message.cs`  
Class: `MessageClass`  
Columns: **Id**, **Content**

### 2. Table

File: `Product.Infrastructure/Data/ProductDbContext.cs`

- `public DbSet<MessageClass> Messages { get; set; }`
- `ToTable("Messages")`

Database file: `Product.Api/app.db`  
Table name: **Messages**

### 3. Repository

| What | File |
|------|------|
| Interface | `Product.Domain/Abstract/IMessageRepository.cs` |
| Class (INSERT) | `Product.Infrastructure/Repositories/MessageRepository.cs` |

### 4. Service

| What | File |
|------|------|
| Interface | `Product.Application/Abstract/IMessageService.cs` |
| Class | `Product.Application/Default/MessageService.cs` |

### 5. API models + controller

| What | File |
|------|------|
| Response JSON | `Product.Api/Models/MessageDto.cs` (Id, Content) |
| Create JSON | `Product.Api/Models/MessageCreateDto.cs` (Content only) |
| **Controller** | `Product.Api/Controllers/MessageController.cs` |

Controller routes:

- `GET /api/message` — list  
- `GET /api/message/{id}` — one row  
- `POST /api/message` — insert  

### 6. Register API

File: `Product.Api/Program.cs`

```csharp
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IMessageService, MessageService>();
```

Also seed Messages if table empty (same file).

### 7. Gateway

File: `APIGateway/ocelot.json`  
Routes: `/api/message` and `/api/message/{everything}` → port 5033

---

## Part C — After model + API complete: Blazor files to create

Only after `MessageController` works in Swagger, create these **BlazorApp** files.

| # | Create this file | Why |
|---|------------------|-----|
| 1 | `BlazorApp/UnitOfWork/messageService.cs` | Calls `api/message` (GET + POST) |
| 2 | `BlazorApp/Program.cs` (edit, not new) | `AddHttpClient<MessageClientService>` base `http://localhost:5083/` |
| 3 | `BlazorApp/Components/Pages/Message.razor` | List page `/messages` (table Id + Content) |
| 4 | `BlazorApp/Components/Pages/MessageCreate.razor` | Create page `/messages/create` (form Content) |
| 5 | `BlazorApp/Components/Layout/NavMenu.razor` (edit) | Menu: Messages, Create Message |

Optional style files (Product already has these; Message reuses `wwwroot/app.css`):

| File | Used by |
|------|---------|
| `BlazorApp/wwwroot/app.css` | Shared list/form look |
| `BlazorApp/Components/Pages/Product.razor.css` | Product table extra styles |
| `BlazorApp/Components/Pages/ProductCreate.razor.css` | Product form extra styles |

No extra `.css` file is required for Message if you use the same CSS classes (`product-page`, `product-table`, `form-actions`).

---

## One picture: Message files top to bottom

```
1  Product.Domain/Entities/Message.cs
2  Product.Domain/Abstract/IMessageRepository.cs
3  Product.Infrastructure/Data/ProductDbContext.cs     ← table Messages
4  Product.Infrastructure/Repositories/MessageRepository.cs
5  Product.Application/Abstract/IMessageService.cs
6  Product.Application/Default/MessageService.cs
7  Product.Api/Models/MessageDto.cs
8  Product.Api/Models/MessageCreateDto.cs
9  Product.Api/Controllers/MessageController.cs        ← API
10 Product.Api/Program.cs                              ← register + seed
11 APIGateway/ocelot.json

--- STOP. Test API: http://localhost:5033/swagger  ---

12 BlazorApp/UnitOfWork/messageService.cs
13 BlazorApp/Program.cs
14 BlazorApp/Components/Pages/Message.razor            ← list UI
15 BlazorApp/Components/Pages/MessageCreate.razor      ← create UI
16 BlazorApp/Components/Layout/NavMenu.razor
```

---

## Blazor pages in this app (all)

| Page file | URL | Calls API |
|-----------|-----|-----------|
| `Home.razor` | `/` | Product count |
| `Product.razor` | `/products` | GET `api/product` |
| `ProductCreate.razor` | `/products/create` | POST `api/product` |
| `Message.razor` | `/messages` | GET `api/message` |
| `MessageCreate.razor` | `/messages/create` | POST `api/message` |

Layout (not a page, but required):

| File | Job |
|------|-----|
| `BlazorApp/Components/App.razor` | HTML shell |
| `BlazorApp/Components/Routes.razor` | Router |
| `BlazorApp/Components/Layout/MainLayout.razor` | Sidebar + body |
| `BlazorApp/Components/Layout/NavMenu.razor` | Left menu |
| `BlazorApp/Components/_Imports.razor` | Shared usings (`BlazorApp.UnitOfWork`) |

---

## Next new feature in THIS project

Copy **Message**, not a made-up name.

Create the same 9 backend files, then the same 4–5 Blazor files.

Example: if the new name is `Order`:

**Backend (create first)**

1. `Product.Domain/Entities/Order.cs`
2. `Product.Domain/Abstract/IOrderRepository.cs`
3. Edit `Product.Infrastructure/Data/ProductDbContext.cs` (add `DbSet` + table)
4. `Product.Infrastructure/Repositories/OrderRepository.cs`
5. `Product.Application/Abstract/IOrderService.cs`
6. `Product.Application/Default/OrderService.cs`
7. `Product.Api/Models/OrderDto.cs`
8. `Product.Api/Models/OrderCreateDto.cs`
9. `Product.Api/Controllers/OrderController.cs`
10. Edit `Product.Api/Program.cs`
11. Edit `APIGateway/ocelot.json`

**After API works — Blazor (create these)**

1. `BlazorApp/UnitOfWork/orderService.cs`
2. Edit `BlazorApp/Program.cs`
3. `BlazorApp/Components/Pages/Order.razor`
4. `BlazorApp/Components/Pages/OrderCreate.razor`
5. Edit `BlazorApp/Components/Layout/NavMenu.razor`

Then stop apps, delete `Product.Api/app.db`, run `scripts/start-all.ps1`.

---

## How to run after files exist

```powershell
cd d:\Project\MyApp\MyApp
powershell -ExecutionPolicy Bypass -File .\scripts\start-all.ps1
```

| App | URL |
|-----|-----|
| Blazor | http://localhost:5062 |
| Product list | http://localhost:5062/products |
| Message list | http://localhost:5062/messages |
| Swagger | http://localhost:5033/swagger |

Project overview: `Docs/PROJECT-GUIDE.md`
