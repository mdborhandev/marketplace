# market - CLI Solution Architecture Setup Specification

## Task

Act as a senior ASP.NET Core architect and create the initial technical foundation for **market**, a Bangladesh-focused multi-vendor fashion marketplace.

Your task is to create the solution, projects, folder structure, configuration, technical infrastructure, documentation, and tests described below.

**This phase is architecture and scaffolding only. Do not implement marketplace business features.**

Before making changes:
1. Inspect the current working directory and existing files.
2. Identify any existing solution or project files.
3. Do not overwrite or delete unrelated work.
4. If a conflicting project structure already exists, explain the conflict and preserve existing files unless a change is essential.

---

## 1. Architecture decision

Use a **traditional layered monolithic architecture**, following the practical workflow of the existing Halda application, but with improved organization and automated test projects.

Do not use:
- Clean Architecture project separation.
- Microservices.
- CQRS or MediatR.
- Unnecessary abstractions or frameworks.
- A separate Web API project.
- A separate Shared project.
- Angular, React, or another frontend framework.

The same ASP.NET Core application must host both the MVC website and the versioned REST API.

Target:
- .NET 10
- C# 14
- PostgreSQL
- EF Core 10 with Npgsql
- ASP.NET Core MVC and Web API
- Razor Views
- Bootstrap 5
- JavaScript

Check package compatibility with the target framework before installing packages. Use stable compatible versions, not preview packages unless there is a documented technical requirement.

---

## 2. Solution and projects

Create these six projects.

### Production projects

1. `market.Application`
   - ASP.NET Core application entry point.
   - MVC controllers and Razor Views.
   - REST API controllers.
   - Middleware, filters, authentication configuration, Swagger, API versioning, and application startup.

2. `market.Core`
   - Models and future module model folders.
   - DTO folders.
   - Interfaces and abstractions.
   - Enums, constants, contracts, and common types.
   - Must not reference any other solution project.

3. `market.DataAccess`
   - EF Core DbContext and database configuration.
   - Repository implementations.
   - Service implementations.
   - Unit of Work.
   - Database migrations and query infrastructure.
   - References `market.Core`.

4. `market.Utilities`
   - Reusable technical helpers.
   - Email, SMS, file management, payment integration helpers, logging, and extensions.
   - Must remain independent of the other solution projects unless a specific dependency is justified.

### Test projects

5. `market.UnitTests`
   - xUnit foundation.
   - Unit-test project references only what it needs.

6. `market.IntegrationTests`
   - ASP.NET Core integration-test foundation.
   - Test host, fixtures, and integration test organization.
   - References the application and required production projects.

### Project dependency rules

- `market.Core`: no project references.
- `market.DataAccess` -> `market.Core`.
- `market.Utilities`: independent.
- `market.Application` -> `market.Core`, `market.DataAccess`, `market.Utilities`.
- Test projects reference only the production projects required for their tests.
- No circular project references.

Do not create additional production projects.

---

## 3. Required project structure

Use this structure as the target organization. Adjust only where the framework or tooling requires it, and document any meaningful deviation.

```text
market/
│
├── market.sln
├── Directory.Build.props
├── Directory.Packages.props
├── .editorconfig
├── .gitignore
├── README.md
├── global.json
│
├── src/
│   ├── market.Application/
│   │   ├── Controllers/
│   │   │   ├── BaseController.cs
│   │   │   └── HomeController.cs
│   │   ├── APIControllers/
│   │   │   ├── V1/
│   │   │   └── V2/
│   │   ├── Areas/
│   │   │   ├── Admin/
│   │   │   ├── Vendor/
│   │   │   └── Customer/
│   │   ├── Views/
│   │   │   ├── Shared/
│   │   │   ├── Home/
│   │   │   └── _ViewImports.cshtml
│   │   ├── wwwroot/
│   │   │   ├── css/
│   │   │   ├── js/
│   │   │   ├── images/
│   │   │   ├── lib/
│   │   │   └── uploads/
│   │   ├── Middleware/
│   │   ├── Filters/
│   │   ├── Extensions/
│   │   ├── Configuration/
│   │   ├── Authentication/
│   │   ├── Swagger/
│   │   ├── ViewModels/
│   │   ├── Properties/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   │   └── appsettings.Development.json
│   │
│   ├── market.Core/
│   │   ├── Models/
│   │   │   ├── Common/
│   │   │   ├── Identity/
│   │   │   ├── Catalog/
│   │   │   ├── Inventory/
│   │   │   ├── Vendor/
│   │   │   ├── Order/
│   │   │   ├── Payment/
│   │   │   └── Shipping/
│   │   ├── DTOs/
│   │   │   ├── Common/
│   │   │   ├── Requests/
│   │   │   └── Responses/
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   ├── Services/
│   │   │   └── Infrastructure/
│   │   ├── Enums/
│   │   ├── Constants/
│   │   ├── Contracts/
│   │   │   ├── API/
│   │   │   └── Pagination/
│   │   └── Common/
│   │       ├── Exceptions/
│   │       └── Results/
│   │
│   ├── market.DataAccess/
│   │   ├── Context/
│   │   ├── Configurations/
│   │   ├── Repositories/
│   │   │   ├── Base/
│   │   │   └── Implementations/
│   │   ├── Services/
│   │   │   └── Implementations/
│   │   ├── UnitOfWork/
│   │   ├── Queries/
│   │   ├── Migrations/
│   │   ├── Extensions/
│   │   └── DependencyInjection/
│   │
│   └── market.Utilities/
│       ├── Email/
│       ├── SMS/
│       ├── FileManagement/
│       ├── Payment/
│       ├── Logging/
│       ├── Helpers/
│       ├── Extensions/
│       └── Common/
│
├── tests/
│   ├── market.UnitTests/
│   │   ├── Services/
│   │   ├── Repositories/
│   │   └── Common/
│   └── market.IntegrationTests/
│       ├── API/
│       ├── MVC/
│       ├── Database/
│       └── Fixtures/
│
├── docs/
│   ├── ARCHITECTURE.md
│   ├── PROJECT_STRUCTURE.md
│   ├── DATABASE_DESIGN.md
│   ├── API_STANDARDS.md
│   ├── API_VERSIONING.md
│   ├── AUTHENTICATION.md
│   ├── CODING_STANDARDS.md
│   └── DEVELOPMENT_GUIDELINES.md
│
└── docker/
    ├── docker-compose.yml
    └── postgres/
```

Create future module folders for organization, but keep them empty or add `.gitkeep` files. Do not add meaningless placeholder classes just to make folders appear in source control.

---

## 4. Architecture and request flow

The intended shared workflow is:

```text
MVC Controller / API Controller
             |
             v
     Service Interface
          (Core)
             |
             v
   Service Implementation
        (DataAccess)
             |
             v
    Repository Interface
          (Core)
             |
             v
  Repository Implementation
        (DataAccess)
             |
             v
        Unit of Work
             |
             v
        EF Core DbContext
             |
             v
         PostgreSQL
```

MVC and API controllers must use the same application service abstractions. Do not duplicate business logic between MVC and API.

For this scaffolding phase, create only the generic technical foundations needed for the architecture. Do not create business services or business repositories.

---

## 5. MVC website foundation

Configure ASP.NET Core MVC with Razor Views.

Prepare:
- MVC controller organization.
- A minimal Home controller and basic view only if needed to verify the MVC application starts.
- Shared layout and view imports.
- Static asset folders for CSS, JavaScript, images, and libraries.
- Bootstrap 5 integration.
- Area routing foundations for Admin, Vendor, and Customer.
- Secure cookie authentication registration for browser-based MVC authentication.

Do not build marketplace pages, dashboards, login pages, or CRUD screens.

---

## 6. Versioned REST API foundation for mobile apps

The application must support both the MVC website and REST API from the same host.

Use URL-segment API versioning:

- `/api/v1/{controller}`
- `/api/v2/{controller}`

Use compatible stable versions of:
- `Asp.Versioning.Mvc`
- `Asp.Versioning.Mvc.ApiExplorer`

Configure:
- API version reader using URL segments.
- API version reporting.
- Explicit default-version behavior.
- Swagger/OpenAPI document grouping by API version.
- JWT Bearer security documentation in Swagger.
- Consistent JSON serialization conventions.
- Global API exception handling.
- ProblemDetails support.
- Consistent model-validation error responses.
- Generic API response contract(s).
- Generic pagination request/response contract(s).
- CancellationToken conventions.
- Authorization extension points.
- Rate-limiting extension point.

Organize future API controllers under:
- `APIControllers/V1/`
- `APIControllers/V2/`

Do not create sample marketplace controllers or endpoints. Do not create Product, Vendor, Cart, Order, or other business API examples.

Do not claim that `/api/v1` by itself is a working endpoint. It is a versioned route prefix; actual routes will exist when feature controllers are implemented.

---

## 7. Authentication foundation

Prepare separate authentication schemes:

### MVC browser authentication
- Secure cookie authentication registration.
- Appropriate cookie security settings.
- HttpOnly and secure cookie configuration.
- Sensible SameSite configuration.

### REST API/mobile authentication
- JWT Bearer authentication registration.
- Issuer, audience, signing key, and expiry configuration placeholders.
- Validate issuer, audience, signing key, and token lifetime.
- Keep secrets out of source control.
- Read sensitive settings from environment variables or development secrets.

Do not implement:
- Login or registration.
- OTP verification.
- Token generation.
- Refresh-token storage or workflows.
- User identity business features.

Document the intended future authentication flow in `docs/AUTHENTICATION.md`.

---

## 8. Data access foundation

Configure:
- PostgreSQL connection through configuration/environment variables.
- EF Core 10 and compatible Npgsql provider.
- DbContext registration.
- Entity configuration discovery.
- Migration assembly/configuration.
- Dependency injection extension methods.
- Unit of Work technical foundation.
- Generic repository infrastructure only if it provides clear value.
- Optional Dapper infrastructure only if genuinely needed; do not add it merely for decoration.

Do not create business entities, business tables, or seed data. Do not create an initial migration containing marketplace tables.

A technical DbContext shell is acceptable, but it must not contain fake business DbSets.

---

## 9. Utilities and logging

Prepare reusable technical folders and configuration for:
- Email integration helpers.
- SMS integration helpers.
- File management helpers.
- Payment integration extension points.
- Logging.
- Common helpers and extensions.

Use Serilog for structured application logging if compatible with the target framework.

Do not integrate real third-party providers or add credentials. Do not implement payment, email, SMS, or file-upload business workflows.

---

## 10. Tests

Create:
- `market.UnitTests` using xUnit.
- `market.IntegrationTests` using xUnit and the appropriate ASP.NET Core test host packages.

Prepare a clean test foundation and example infrastructure tests only where they verify technical setup, such as application startup or basic routing configuration.

Do not create tests for nonexistent business features.

---

## 11. Configuration, packages, and local development

Set up:
- `Directory.Packages.props` for central NuGet package version management.
- `Directory.Build.props` for shared build settings.
- `.editorconfig`.
- `.gitignore`.
- `global.json` only if the required SDK version is available in the environment; do not pin the repository to an unavailable SDK.
- Development and production configuration separation.
- Environment-variable based secrets and connection strings.
- Docker Compose for local PostgreSQL.
- Health-check infrastructure.

Redis is optional and should not be added unless there is a clear technical need.

Do not add unnecessary dependencies. Explain the purpose of every major package in the final report.

---

## 12. Documentation

Create the following Markdown documentation:

### `docs/ARCHITECTURE.md`
Explain the layered monolith, project responsibilities, request flow, and dependency rules.

### `docs/PROJECT_STRUCTURE.md`
Document the complete folder tree and the purpose of each project and major folder.

### `docs/DATABASE_DESIGN.md`
Document PostgreSQL, EF Core, DbContext, configuration, migrations, and future database conventions. Clearly state that business tables are not implemented yet.

### `docs/API_STANDARDS.md`
Document REST conventions, response contracts, errors, validation, pagination, JSON conventions, and CancellationToken usage.

### `docs/API_VERSIONING.md`
Document URL-segment versioning, version discovery, Swagger grouping, compatibility, and future deprecation rules.

### `docs/AUTHENTICATION.md`
Document the separation between MVC cookie authentication and API JWT Bearer authentication, configuration placeholders, and future token workflow.

### `docs/CODING_STANDARDS.md`
Document naming, folder organization, dependency rules, async conventions, and code formatting.

### `docs/DEVELOPMENT_GUIDELINES.md`
Document local setup, configuration, database startup, build, test, and run commands.

Keep documentation consistent with the actual implementation. Do not document features that were not configured.

---

## 13. Strict scope restrictions

This task is only for solution architecture and technical scaffolding.

**Do not implement marketplace business logic.**

Do not create:
- Product entities or sample Product features.
- Category, brand, size, color, or variant entities.
- Vendor or customer business entities.
- Cart or checkout features.
- Order, payment, or shipping business entities.
- Business DTO implementations.
- Business services or business repositories.
- Business controllers or business API endpoints.
- CRUD operations.
- Business workflows.
- Business database tables.
- Marketplace seed data.
- Fake sample data.
- Unnecessary placeholder classes.

Future module folders are allowed, but they must remain empty or contain only `.gitkeep` files.

Only create minimal technical infrastructure needed to prove that the solution builds and starts.

Do not overwrite unrelated existing files.

---

## 14. Execution plan

Work in this order:

1. Inspect the repository and available .NET SDK.
2. Confirm the .NET 10 SDK is available. If it is not, report the issue before selecting another target framework.
3. Create the solution and six projects.
4. Configure project references and shared build/package settings.
5. Create the required folder structure.
6. Add compatible NuGet packages.
7. Configure MVC, Razor Views, and static assets.
8. Configure API versioning and Swagger.
9. Register cookie and JWT authentication separately.
10. Configure PostgreSQL and the EF Core DbContext shell.
11. Add middleware, ProblemDetails, health checks, and dependency injection.
12. Add logging and Docker Compose for PostgreSQL.
13. Create test foundations.
14. Write all required documentation.
15. Restore packages, build, test, and fix errors.

Do not stop after writing a proposed plan. Carry out the implementation in the current repository, subject to the scope restrictions above.

---

## 15. Acceptance criteria

The task is complete only when:

- The six projects exist in the correct solution folders.
- Project references follow the dependency rules.
- No circular references exist.
- The solution targets .NET 10 / C# 14.
- MVC and Razor Views are configured.
- Versioned REST API infrastructure is registered.
- Swagger groups API documentation by version.
- Cookie and JWT authentication schemes are separately registered.
- PostgreSQL and EF Core configuration are environment-based.
- Technical middleware and health checks are registered.
- Docker Compose can start the local PostgreSQL service.
- Unit and integration test projects build.
- All required documentation files exist and match the implementation.
- The full solution restores and builds successfully.
- Tests run successfully, or any environment-related limitation is clearly reported.
- No marketplace business feature, business entity, business endpoint, business table, or fake sample data has been introduced.

---

## 16. Final report

At completion, provide:

1. Summary of what was created.
2. Final solution and project tree.
3. Project dependency diagram.
4. Important NuGet packages and their purposes.
5. MVC and REST API configuration summary.
6. API versioning and Swagger configuration summary.
7. Authentication scheme configuration summary.
8. PostgreSQL and EF Core configuration summary.
9. Build output and test results.
10. Any unresolved issues or environment limitations.

Be accurate. Do not claim that something works unless you actually verified it.
