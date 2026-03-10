# MCV Store — ASP.NET Core MVC E-Commerce Application

A Simple e-commerce web application built with **ASP.NET Core MVC (.NET 10)**, **Entity Framework Core**, and **SQL Server**. Developed as an ITI training project demonstrating clean architecture, identity management, and real-world shopping workflows.

---

## Features

### Customer-Facing
- **Product Catalog** — Browse, search, filter by category, sort, and paginate products
- **Product Details** — View product info with stock availability
- **Shopping Cart** — Add/update/remove items with real-time stock validation
- **Checkout** — Select a shipping address and place orders
- **Order History** — View past orders with status tracking and details
- **Address Management** — CRUD operations for shipping addresses with default address support

### Admin Panel
- **Dashboard** — Central hub for all admin operations
- **Manage Products** — Create, edit, delete, and toggle active/inactive products
- **Manage Categories** — Hierarchical categories with parent-child relationships
- **Manage Orders** — View all orders and update order status (Pending → Processing → Shipped → Delivered / Cancelled)
- **Manage Users** — View users and assign/remove roles

### Authentication & Authorization
- **ASP.NET Identity** — Registration, login, logout with password policies
- **Role-Based Access** — Admin, SubAdmin, and Customer roles
- **`[Authorize]`** — Protected controllers for cart, orders, and admin areas

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core MVC (.NET 10) |
| **ORM** | Entity Framework Core 10 |
| **Database** | SQL Server |
| **Authentication** | ASP.NET Core Identity |
| **Frontend** | Razor Views, Bootstrap 5 |
| **Architecture** | Repository + Unit of Work Pattern |

---

## Project Structure

```
ITI-MVC-Project/
├── Entities/                          # Data layer
│   ├── Data/
│   │   └── AppDbContext.cs            # EF Core context, relationships, query filters, soft delete
│   ├── Models/
│   │   ├── Base.cs                    # BaseEntity (Id, IsDeleted, CreatedAt, UpdatedAt)
│   │   ├── User.cs                    # IdentityUser extension
│   │   ├── Product.cs
│   │   ├── Category.cs                # Self-referencing parent/child
│   │   ├── Order.cs                   # OrderStatus enum
│   │   ├── OrderItem.cs
│   │   ├── Cart.cs                    # One-to-one with User
│   │   ├── CartItem.cs                # Unique (CartId, ProductId) filtered index
│   │   └── Address.cs                 # IsDefault flag
│   ├── Repositories/
│   │   ├── EntityRepo.cs              # Generic repo with Query(), GetAll(), FindAll()
│   │   └── UnitOfWork.cs              # Aggregates all repos + SaveChanges()
│   └── Migrations/
│
├── MCV/                               # Web layer
│   ├── Controllers/
│   │   ├── HomeController.cs
│   │   ├── AccountController.cs       # Login, Register, Logout, Address CRUD
│   │   ├── Customer/
│   │   │   ├── CatalogController.cs   # Index (filter/sort/page), Details
│   │   │   ├── CartController.cs      # Add, UpdateQuantity, Remove, Clear
│   │   │   └── OrderController.cs     # Checkout, PlaceOrder, Details, Index
│   │   └── Admin/
│   │       ├── AdminController.cs     # Dashboard
│   │       ├── ManagerController.cs   # User role management
│   │       ├── ManageCategoryController.cs
│   │       ├── ManageProductsController.cs
│   │       └── ManageOrdersController.cs
│   ├── ViewModels/
│   │   ├── Account/                   # LoginVM, RegisterVM, CreateAddressVM, EditAddressVM
│   │   ├── Catalog/                   # CatalogVM, ProductDetailsVM
│   │   ├── Cart/                      # CartVM, CartItemVM
│   │   ├── Order/                     # CheckoutVM
│   │   ├── ManageCategory/            # CreateCategoryVM, EditCategoryVM
│   │   ├── ManageProducts/            # CreateProductVM, EditProductVM
│   │   └── ManageOrders/              # OrdersVM
│   ├── Views/                         # Razor views organized by controller
│   └── Program.cs                     # DI, middleware, Identity config
```

---

## Database Schema

```
User (IdentityUser)
 ├── 1:N → Address (IsDefault)
 ├── 1:N → Order
 └── 1:1 → Cart
              └── 1:N → CartItem → Product

Category (self-ref ParentCategory)
 └── 1:N → Product
              └── 1:N → OrderItem → Order
```

### Key Design Decisions
- **Soft Delete** — All entities inherit `BaseEntity.IsDeleted`; `SaveChanges()` intercepts deletes and sets the flag. **Exception:** Cart/CartItem use hard delete (transient data).
- **Global Query Filters** — `HasQueryFilter(e => !e.IsDeleted)` on all entities automatically excludes deleted rows.
- **Filtered Unique Index** — `CartItem(CartId, ProductId)` is unique only where `IsDeleted = 0`, preventing conflicts with soft-deleted rows.
- **Audit Fields** — `CreatedAt` set on insert, `UpdatedAt` set on modify, all handled in `ApplyAuditAndSoftDelete()`.

---

## Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/Bish0y-Adel/ITI_MVC_Project.git
   cd ITI_MVC_Project
   ```

2. **Configure the connection string**

   Update `appsettings.json` in the `MCV` project:
   ```json
   {
     "ConnectionStrings": {
       "Connection1": "Server=.;Database=MCVStoreDb;Trusted_Connection=True;TrustServerCertificate=True;"
     }
   }
   ```

3. **Apply migrations**
   ```bash
   dotnet ef database update --project Entities --startup-project MCV
   ```

4. **Run the application**
   ```bash
   dotnet run --project MCV
   ```

5. **Access the app**
   - Home: `https://localhost:5001`
   - Register a new account, then use the Admin dashboard to assign roles.

### Default Roles (Seeded)
| Role | Purpose |
|------|---------|
| Admin | Full access to all admin features |
| SubAdmin | Limited admin access |
| Customer | Default role for registered users |

---

## Architecture

### Repository + Unit of Work

```
Controller → UnitOfWork → EntityRepo<T> → AppDbContext → SQL Server
```

- **`EntityRepo<T>`** — Generic repository with `Query()` (returns `IQueryable` for deferred execution), `GetAll()`, `FindAll()`, `GetById()`, `Add()`, `Update()`, `Delete()`
- **`UnitOfWork`** — Groups all repos; single `SaveChanges()` call commits all changes atomically
- **`Query()` method** — Returns `IQueryable<T>` for server-side filtering, sorting, and pagination (avoids loading entire tables into memory)

### Order Flow

```
Catalog → Add to Cart → Cart → Checkout (select address) → PlaceOrder → Order Details
                                    │
                                    ├── Validates stock per item
                                    ├── Creates Order + OrderItems
                                    ├── Decrements Product.StockQuantity
                                    └── Clears cart (hard delete)
```

---

## Screenshots

## Products Page
![Products](images/catalog.png)
---

## License

This project was developed as part of the **ITI (Information Technology Institute)** training program.
