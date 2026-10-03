# eShopModernized: ASP.NET WebForms to .NET 8 WebAPI Migration

[![.NET 8](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Build & Test](https://img.shields.io/badge/tests-10%20passed-brightgreen.svg)]()

Modernized enterprise architecture port of Microsoft's legacy **eShopOnWebForms** catalog service to **ASP.NET Core 8 WebAPI, Entity Framework Core 8, and In-Memory/PostgreSQL resiliency**.

---

## Architecture Transformation

| Legacy WebForms Component | Modern .NET 8 Replacement | Architectural Benefits |
|---|---|---|
| `System.Web.UI.Page` (Stateful session/ViewState) | Stateless RESTful WebAPI (`CatalogController`) | Horizontally scalable, cloud-native, sub-millisecond response times |
| `web.config` connection strings | `appsettings.json` + `IOptions<CatalogSettings>` | Twelve-Factor App compliance, environment variable overrides |
| ADO.NET / EF6 ObjectContext | Entity Framework Core 8 `CatalogDbContext` | Async/await, optimized queries, HiLo key generation |
| ASP.NET App_Data hardcoded paths | SQLite / InMemory / Managed SQL Server | Ephemeral test containers, automated CI/CD pipeline |

---

## Solution Structure

```
eShopModernized/
├── eShopModern.Catalog.sln
├── eShopModern.Catalog/
│   ├── Controllers/CatalogController.cs      # REST API endpoints
│   ├── Data/CatalogDbContext.cs             # EF Core DbContext with HiLo sequences
│   ├── Models/CatalogModels.cs              # Clean DTOs and entities
│   ├── Services/CatalogService.cs           # Business logic layer
│   └── Program.cs                           # Dependency injection and middleware
└── eShopModern.Catalog.Tests/
    └── CatalogServiceTests.cs               # Exhaustive xUnit test suite
```

---

## Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Build & Run Tests
```bash
dotnet test
```

### Run WebAPI Server
```bash
dotnet run --project eShopModern.Catalog
```
Navigate to `http://localhost:5151/swagger` (or probe `GET /api/catalog/items`).
