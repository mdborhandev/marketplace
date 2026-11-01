# Project structure

`src/PoshakGhor.Application` contains the ASP.NET Core host, Controllers, APIControllers/V1 and V2, Areas, Views, `wwwroot`, Middleware, Filters, Extensions, Configuration, Authentication, Swagger, and ViewModels. `src/PoshakGhor.Core` contains Models, DTOs, Interfaces, Enums, Constants, Contracts, Common types, and the API/pagination contracts. `src/PoshakGhor.DataAccess` contains Context, Configurations, Repositories, Services, UnitOfWork, Queries, Migrations, Extensions, and DependencyInjection. `src/PoshakGhor.Utilities` contains technical integration/helper folders. `tests` contains unit and integration projects. `docs` holds architecture and development references; `docker` holds local PostgreSQL configuration.

```text
PoshakGhor.sln
├── src
│   ├── PoshakGhor.Application
│   │   ├── Controllers
│   │   ├── APIControllers/{V1,V2}
│   │   ├── Areas/{Admin,Vendor,Customer}
│   │   ├── Views/{Home,Shared}
│   │   ├── wwwroot/{css,js,images,lib,uploads}
│   │   ├── {Middleware,Filters,Extensions,Configuration,Authentication,Swagger,ViewModels}
│   │   └── Program.cs, appsettings*.json
│   ├── PoshakGhor.Core
│   │   ├── Models/{Common,Identity,Catalog,Inventory,Vendor,Order,Payment,Shipping}
│   │   ├── DTOs/{Common,Requests,Responses}
│   │   ├── Interfaces/{Repositories,Services,Infrastructure}
│   │   ├── Contracts/{API,Pagination}
│   │   └── {Enums,Constants,Common}
│   ├── PoshakGhor.DataAccess
│   │   └── {Context,Configurations,Repositories,Services,UnitOfWork,Queries,Migrations,Extensions,DependencyInjection}
│   └── PoshakGhor.Utilities
│       └── {Email,SMS,FileManagement,Payment,Logging,Helpers,Extensions,Common}
├── tests
│   ├── PoshakGhor.UnitTests/{Services,Repositories,Common}
│   └── PoshakGhor.IntegrationTests/{API,MVC,Database,Fixtures}
├── docs
└── docker/{docker-compose.yml,postgres}
```

Repository root also contains `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, `.gitignore`, `global.json`, and `README.md`.

Future feature folders are intentionally empty. The only implemented types support generic API pagination/responses, exception handling, EF Core setup, and unit of work. No marketplace business models, endpoints, or tables exist.
