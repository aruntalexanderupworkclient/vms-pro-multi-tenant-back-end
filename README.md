# VMS Pro - Multi-Tenant Visitor Management System

A multi-tenant Visitor Management System built with **ASP.NET Core 8** following **Clean Architecture** principles.

## Project Structure

```
??? src/
?   ??? VMS.API/              # Web API layer (Controllers, Middleware)
?   ??? VMS.Application/      # Application layer (Services, DTOs, Interfaces, Mappings)
?   ??? VMS.Core/             # Domain layer (Entities, Enums, Interfaces)
?   ??? VMS.Infrastructure/   # Infrastructure layer (EF Core, Data Access, Repositories)
?   ??? VMS.Shared/           # Shared utilities and cross-cutting concerns
??? VMS.sln
??? README.md
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (or your configured database provider)

## Getting Started

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd vms-pro-multi-tenant-back-end
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update the connection string** in `src/VMS.API/appsettings.json`

4. **Apply migrations**
   ```bash
   dotnet ef database update --project src/VMS.Infrastructure --startup-project src/VMS.API
   ```

5. **Run the application**
   ```bash
   dotnet run --project src/VMS.API
   ```

## Architecture

This project follows **Clean Architecture**:

| Layer | Project | Responsibility |
|-------|---------|---------------|
| **Domain** | VMS.Core | Entities, domain logic, interfaces |
| **Application** | VMS.Application | Business logic, DTOs, service interfaces |
| **Infrastructure** | VMS.Infrastructure | EF Core, database context, repositories |
| **Presentation** | VMS.API | API controllers, middleware, configuration |
| **Shared** | VMS.Shared | Cross-cutting concerns, shared utilities |

## License

This project is for study/learning purposes.
