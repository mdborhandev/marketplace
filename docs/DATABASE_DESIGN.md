# Database design

PostgreSQL is accessed through EF Core 10 and Npgsql 10. `marketDbContext` discovers entity configurations in the DataAccess assembly and uses that assembly for future migrations. The context currently has no DbSets or business entities, and no migration or business table has been created.

Configure `ConnectionStrings__PostgreSQL` in the process environment. Never commit credentials. Use `docker compose -f docker/docker-compose.yml up -d` for local PostgreSQL. Add entity configurations and migrations with feature work; review generated SQL before applying migrations outside development. Use async EF operations and pass cancellation tokens.
