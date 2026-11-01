# API versioning

URL-segment versioning uses `/api/v1/{controller}` and `/api/v2/{controller}`. The application reports supported/deprecated versions and publishes versioned API explorer groups as `v1`, `v2`, and so on. Swagger UI lists a document for each discovered API version; no document appears until a versioned API controller exists. `/api/v1` alone is not an endpoint.

The default version is 1.0 for metadata only; unspecified versions are not silently assumed. Add explicit version metadata to controllers. Keep a released contract compatible within its version. For breaking changes, introduce a new version and mark old versions deprecated with a published migration window before removal.
