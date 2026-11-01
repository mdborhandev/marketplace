# Architecture

PoshakGhor is a traditional layered monolith. One ASP.NET Core host serves Razor MVC pages and versioned REST APIs. Controllers share service abstractions as feature services are added.

| Project | Responsibility |
|---|---|
| Application | Host, MVC/API endpoints, middleware, authentication, Swagger, presentation |
| Core | Framework independent contracts, models, interfaces, and common types |
| DataAccess | EF Core, PostgreSQL, persistence implementations, unit of work |
| Utilities | Independent technical helpers and integrations |
| UnitTests | Isolated tests of individual production projects |
| IntegrationTests | HTTP host and cross-project tests |

Dependency flow: Application → Core, DataAccess, Utilities; DataAccess → Core; Utilities → no solution project; Core → no solution project. Tests depend only on the production projects they exercise. No business services or entities are present yet.

Request flow for future features: MVC/API controller → Core service abstraction → DataAccess service → repository abstraction/implementation → unit of work and EF Core → PostgreSQL. This is one deployable application; there are no microservices or separate API host.
