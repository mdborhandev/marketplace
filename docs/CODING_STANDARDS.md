# Coding standards

Use C# 14, nullable reference types, implicit usings, four-space indentation, PascalCase for public types/members, and descriptive names. Keep feature types in their owning project and folders. Core remains framework-independent; DataAccess references Core; Utilities stays independent; Application composes these projects. Avoid circular references, unnecessary abstractions, and business placeholders.

Use async APIs for I/O, propagate `CancellationToken`, and avoid `.Result`/`.Wait()`. Prefer constructor injection and focused types. Keep configuration secrets outside source control. Run `dotnet format market.sln` before submitting changes.
