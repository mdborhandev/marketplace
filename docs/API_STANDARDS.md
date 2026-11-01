# API standards

The API shares the MVC host and uses URL-segment versions. Future controllers should use `/api/v{version}/[controller]`, explicit API version metadata, asynchronous actions, and `CancellationToken` parameters forwarded to dependencies. Return resource-oriented HTTP status codes and JSON using ASP.NET Core's default camelCase conventions.

Use `ApiResponse<T>` for successful envelopes only where a feature contract calls for one. `PagedResponse<T>` carries items, page, page size, and total count; `PaginationRequest` clamps page size to 100. Errors use ProblemDetails; invalid model state returns ValidationProblemDetails with HTTP 400. Global unhandled exceptions are logged and mapped to generic HTTP 500 ProblemDetails. Do not expose exception details in production. Swagger documents JWT bearer authorization. No business endpoints are currently registered.
