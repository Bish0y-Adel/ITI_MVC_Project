<div align="center">

# 🛒 MCV Store

### ASP.NET Core MVC E-Commerce Application

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/en-us/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-ITI-green?style=for-the-badge)](LICENSE)

A full-featured e-commerce web application built with **ASP.NET Core MVC (.NET 10)**, **Entity Framework Core**, and **SQL Server**.  
Developed as an ITI training project demonstrating clean architecture, identity management, and real-world shopping workflows.

---

</div>

## 📋 Table of Contents

- [✨ Features](#-features)
- [🛠️ Tech Stack](#️-tech-stack)
- [📁 Project Structure](#-project-structure)
- [🗄️ Database Schema](#️-database-schema)
- [🚀 Getting Started](#-getting-started)
- [🏗️ Architecture](#️-architecture)
- [📸 Screenshots](#-screenshots)
- [📄 License](#-license)

---

## ✨ Features

### 🛍️ Customer-Facing

| Feature | Description |
|:--------|:------------|
| 📦 **Product Catalog** | Browse, search, filter by category, sort, and paginate products |
| 🔍 **Product Details** | View product info with stock availability |
| 🛒 **Shopping Cart** | Add/update/remove items with real-time stock validation |
| 💳 **Checkout** | Select a shipping address and place orders |
| 📜 **Order History** | View past orders with status tracking and details |
| 📍 **Address Management** | CRUD operations for shipping addresses with default address support |

### 🔧 Admin Panel

| Feature | Description |
|:--------|:------------|
| 📊 **Dashboard** | Central hub for all admin operations |
| 📦 **Manage Products** | Create, edit, delete, and toggle active/inactive products |
| 🏷️ **Manage Categories** | Hierarchical categories with parent-child relationships |
| 📋 **Manage Orders** | View all orders and update order status (Pending → Processing → Shipped → Delivered / Cancelled) |
| 👥 **Manage Users** | View users and assign/remove roles |

### 🔐 Authentication & Authorization

| Feature | Description |
|:--------|:------------|
| 🪪 **ASP.NET Identity** | Registration, login, logout with password policies |
| 🛡️ **Role-Based Access** | Admin, SubAdmin, and Customer roles |
| 🔒 **`[Authorize]`** | Protected controllers for cart, orders, and admin areas |

---

## 🛠️ Tech Stack

<div align="center">

| Layer | Technology | Badge |
|:------|:-----------|:------|
| **Framework** | ASP.NET Core MVC (.NET 10) | ![.NET](https://img.shields.io/badge/.NET_10-512BD4?style=flat-square&logo=dotnet&logoColor=white) |
| **ORM** | Entity Framework Core 10 | ![EF Core](https://img.shields.io/badge/EF_Core_10-512BD4?style=flat-square&logo=dotnet&logoColor=white) |
| **Database** | SQL Server | ![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white) |
| **Authentication** | ASP.NET Core Identity | ![Identity](https://img.shields.io/badge/Identity-512BD4?style=flat-square&logo=dotnet&logoColor=white) |
| **Frontend** | Razor Views, Bootstrap 5 | ![Bootstrap](https://img.shields.io/badge/Bootstrap_5-7952B3?style=flat-square&logo=bootstrap&logoColor=white) |
| **Architecture** | Repository + Unit of Work | ![Pattern](https://img.shields.io/badge/UoW_Pattern-2D9CDB?style=flat-square&logo=buffer&logoColor=white) |

</div>

---

## 📁 Project Structure

```
📦 ITI-MVC-Project/
├── 📂 Entities/                          # 🗃️ Data layer
│   ├── 📂 Data/
│   │   └── 📄 AppDbContext.cs            # EF Core context, relationships, query filters, soft delete
│   ├── 📂 Models/
│   │   ├── 📄 Base.cs                    # BaseEntity (Id, IsDeleted, CreatedAt, UpdatedAt)
│   │   ├── 📄 User.cs                    # IdentityUser extension
│   │   ├── 📄 Product.cs
│   │   ├── 📄 Category.cs                # Self-referencing parent/child
│   │   ├── 📄 Order.cs                   # OrderStatus enum
│   │   ├── 📄 OrderItem.cs
│   │   ├── 📄 Cart.cs                    # One-to-one with User
│   │   ├── 📄 CartItem.cs                # Unique (CartId, ProductId) filtered index
│   │   └── 📄 Address.cs                 # IsDefault flag
│   ├── 📂 Repositories/
│   │   ├── 📄 EntityRepo.cs              # Generic repo with Query(), GetAll(), FindAll()
│   │   └── 📄 UnitOfWork.cs              # Aggregates all repos + SaveChanges()
│   └── 📂 Migrations/
│
├── 📂 MCV/                               # 🌐 Web layer
│   ├── 📂 Controllers/
│   │   ├── 📄 HomeController.cs
│   │   ├── 📄 AccountController.cs       # Login, Register, Logout, Address CRUD
│   │   ├── 📂 Customer/
│   │   │   ├── 📄 CatalogController.cs   # Index (filter/sort/page), Details
│   │   │   ├── 📄 CartController.cs      # Add, UpdateQuantity, Remove, Clear
│   │   │   └── 📄 OrderController.cs     # Checkout, PlaceOrder, Details, Index
│   │   └── 📂 Admin/
│   │       ├── 📄 AdminController.cs     # Dashboard
│   │       ├── 📄 ManagerController.cs   # User role management
│   │       ├── 📄 ManageCategoryController.cs
│   │       ├── 📄 ManageProductsController.cs
│   │       └── 📄 ManageOrdersController.cs
│   ├── 📂 ViewModels/                    # View-specific data models
│   ├── 📂 Views/                         # Razor views organized by controller
│   └── 📄 Program.cs                     # DI, middleware, Identity config
```

---

## 🗄️ Database Schema

```
👤 User (IdentityUser)
 ├── 1:N → 📍 Address (IsDefault)
 ├── 1:N → 📋 Order
 └── 1:1 → 🛒 Cart
                └── 1:N → 📦 CartItem → Product

🏷️ Category (self-ref ParentCategory)
 └── 1:N → 📦 Product
                └── 1:N → 📋 OrderItem → Order
```

### 🔑 Key Design Decisions

> | Decision | Details |
> |:---------|:--------|
> | 🗑️ **Soft Delete** | All entities inherit `BaseEntity.IsDeleted`; `SaveChanges()` intercepts deletes and sets the flag. **Exception:** Cart/CartItem use hard delete (transient data). |
> | 🔍 **Global Query Filters** | `HasQueryFilter(e => !e.IsDeleted)` on all entities automatically excludes deleted rows. |
> | 🔗 **Filtered Unique Index** | `CartItem(CartId, ProductId)` is unique only where `IsDeleted = 0`, preventing conflicts with soft-deleted rows. |
> | 🕐 **Audit Fields** | `CreatedAt` set on insert, `UpdatedAt` set on modify, all handled in `ApplyAuditAndSoftDelete()`. |

---

## 🚀 Getting Started

### 📋 Prerequisites

| Requirement | Link |
|:------------|:-----|
| ![.NET](https://img.shields.io/badge/.NET_10_SDK-512BD4?style=flat-square&logo=dotnet&logoColor=white) | [Download .NET 10](https://dotnet.microsoft.com/download) |
| ![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=flat-square&logo=microsoftsqlserver&logoColor=white) | [Download SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance) |

### ⚙️ Setup

**1️⃣ Clone the repository**
```bash
git clone https://github.com/Bish0y-Adel/ITI_MVC_Project.git
cd ITI_MVC_Project
```

**2️⃣ Configure the connection string**

Update `appsettings.json` in the `MCV` project:
```json
{
  "ConnectionStrings": {
    "Connection1": "Server=.;Database=MCVStoreDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**3️⃣ Apply migrations**
```bash
dotnet ef database update --project Entities --startup-project MCV
```

**4️⃣ Run the application**
```bash
dotnet run --project MCV
```

**5️⃣ Access the app**
- 🌐 Home: `https://localhost:5001`
- 👤 Register a new account, then use the Admin dashboard to assign roles.

### 👥 Default Roles (Seeded)

| Role | Badge | Purpose |
|:-----|:------|:--------|
| **Admin** | ![Admin](https://img.shields.io/badge/Admin-DC3545?style=flat-square) | Full access to all admin features |
| **SubAdmin** | ![SubAdmin](https://img.shields.io/badge/SubAdmin-FFC107?style=flat-square) | Limited admin access |
| **Customer** | ![Customer](https://img.shields.io/badge/Customer-28A745?style=flat-square) | Default role for registered users |

---

## 🏗️ Architecture

### 🔄 Repository + Unit of Work

```
🎮 Controller  →  📦 UnitOfWork  →  🗃️ EntityRepo<T>  →  💾 AppDbContext  →  🗄️ SQL Server
```

| Component | Description |
|:----------|:------------|
| 🗃️ **`EntityRepo<T>`** | Generic repository with `Query()` (returns `IQueryable` for deferred execution), `GetAll()`, `FindAll()`, `GetById()`, `Add()`, `Update()`, `Delete()` |
| 📦 **`UnitOfWork`** | Groups all repos; single `SaveChanges()` call commits all changes atomically |
| 🔍 **`Query()` method** | Returns `IQueryable<T>` for server-side filtering, sorting, and pagination (avoids loading entire tables into memory) |

### 🛒 Order Flow

```
📦 Catalog  →  ➕ Add to Cart  →  🛒 Cart  →  💳 Checkout (select address)  →  ✅ PlaceOrder  →  📋 Order Details
                                                        │
                                                        ├── ✔️ Validates stock per item
                                                        ├── 📝 Creates Order + OrderItems
                                                        ├── 📉 Decrements Product.StockQuantity
                                                        └── 🗑️ Clears cart (hard delete)
```

---

## 📸 Screenshots

### 🛍️ Customer Experience

<details>
<summary><b>📦 Product Catalog</b></summary>
<br>

![Product Catalog](images/catalog.png)

</details>

<details>
<summary><b>🔍 Product Details</b></summary>
<br>

![Product Details](images/ProductDetails.png)

</details>

<details>
<summary><b>🛒 Shopping Cart</b></summary>
<br>

![Shopping Cart](images/cart.png)

</details>

<details>
<summary><b>📋 Orders</b></summary>
<br>

![Orders](images/orders.png)

</details>

<details>
<summary><b>📄 Order Details</b></summary>
<br>

![Order Details](images/orderDetails.png)

</details>

<details>
<summary><b>📍 Addresses</b></summary>
<br>

![Addresses](images/addresses.png)

</details>

---

### 🔧 Admin Panel

<details>
<summary><b>📦 Product Management</b></summary>
<br>

![Product Management](images/AdminProductManagment.png)

</details>

<details>
<summary><b>➕ Create Product</b></summary>
<br>

![Create Product](images/CreateProduct.png)

</details>

<details>
<summary><b>🏷️ Categories</b></summary>
<br>

![Categories](images/categories.png)

</details>

<details>
<summary><b>✏️ Edit Category</b></summary>
<br>

![Edit Category](images/editcategory.png)

</details>

<details>
<summary><b>📋 Manage Orders</b></summary>
<br>

![Manage Orders](images/AdminManageOrders.png)

</details>

<details>
<summary><b>👥 User Management</b></summary>
<br>

![User Management](images/AdminUserManagment.png)

</details>

---

## 📄 License

This project was developed as part of the **ITI (Information Technology Institute)** training program.

---

<div align="center">

**⭐ If you found this project helpful, give it a star!**

Made with ❤️ for ITI

</div>
