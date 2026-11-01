# Development guidelines

Requirements: .NET SDK 10.0.112 or a later 10.0 patch, Docker Compose, and PostgreSQL for database-backed work. Bootstrap 5.3.8 assets are referenced from jsDelivr with integrity attributes. Restore/build/test from the repository root:

```sh
dotnet restore PoshakGhor.sln
dotnet build PoshakGhor.sln
dotnet test PoshakGhor.sln
```

Start local PostgreSQL with `docker compose -f docker/docker-compose.yml up -d`. Supply `ConnectionStrings__PostgreSQL` and JWT environment variables as documented in `AUTHENTICATION.md`. Run the application with `dotnet run --project src/PoshakGhor.Application`. Swagger is enabled in Development; `/health` reports database connectivity. No migration is needed for the empty technical DbContext. The tests currently verify only technical startup/routing foundations.
