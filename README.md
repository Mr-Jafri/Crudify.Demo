# Crudify Solution Structure

This document outlines the folder structure of the **Crudify** solution, organized by layer and responsibility.

---

## 📁 Solution Items

### 📁 src
Contains the main application source code.

#### 📁 Crudify.Api
The entry point of the application (Web API).

- 📁 Controllers
- 📁 Middlewares
- 📄 appsettings.json
- 📄 Crudify.Api.http
- 📄 Dockerfile
- 📄 GlobalUsings.cs
- 📄 Program.cs

#### 📁 Crudify.Application
Application-level logic such as DTOs, interfaces, and services.

- 📁 Dependencies
- 📁 Dtos
- 📁 Interfaces
- 📁 Services
- 📄 GlobalUsings.cs

#### 📁 Crudify.Domain
Contains domain entities and core business models.

- 📁 Entities
- 📄 GlobalUsings.cs

#### 📁 Crudify.Infrastructure
Manages persistence, logging, and other infrastructure-level concerns.

- 📁 Database
- 📁 Extensions
- 📁 FluentConfigurations
  - 📄 ActivityLogConfiguration.cs
  - 📄 ApplicationUserConfiguration.cs
  - 📄 ExceptionLogConfiguration.cs
  - 📄 RefreshTokenConfiguration.cs
  - 📄 SeedingEntryConfiguration.cs
- 📁 Interfaces
- 📁 Migrations
- 📄 ApplicationContextFactory.cs
- 📄 GlobalUsings.cs
- 📄 LoggingService.cs

---

## 🧪 tests

### 📁 unit
Contains unit test projects.

- 📁 Crudify.Api.Tests
- 📁 Crudify.Application.Tests

---

## 📝 Notes

- The solution follows a clean architecture pattern.
- Each project is structured to follow a clear separation of concerns.
- All configurations are split logically for maintainability and scalability.

---

## ⚙️ Environment Setup

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/)
- Optional: Postman or any HTTP client for testing endpoints
