# Project structure

`src/market.Application` contains the ASP.NET Core host, Controllers, APIControllers/V1 and V2, Areas, Views, `wwwroot`, Middleware, Filters, Extensions, Configuration, Authentication, Swagger, and ViewModels. `src/market.Core` contains Models, DTOs, Interfaces, Enums, Constants, Contracts, Common types, and the API/pagination contracts. `src/market.DataAccess` contains Context, Configurations, Repositories, Services, UnitOfWork, Queries, Migrations, Extensions, and DependencyInjection. `src/market.Utilities` contains technical integration/helper folders. `tests` contains unit and integration projects. `docs` holds architecture and development references; `docker` holds local PostgreSQL configuration.

```text
market.sln
├── src
│   ├── market.Application
│   │   ├── Controllers
│   │   ├── APIControllers/{V1,V2}
│   │   ├── Areas/{Admin,Vendor,Customer}
│   │   ├── Views/{Home,Shared}
│   │   ├── wwwroot/{css,js,images,lib,uploads}
│   │   ├── {Middleware,Filters,Extensions,Configuration,Authentication,Swagger,ViewModels}
│   │   └── Program.cs, appsettings*.json
│   ├── market.Core
│   │   ├── Models/{Common,Identity,Catalog,Inventory,Vendor,Order,Payment,Shipping}
│   │   ├── DTOs/{Common,Requests,Responses}
│   │   ├── Interfaces/{Repositories,Services,Infrastructure}
│   │   ├── Contracts/{API,Pagination}
│   │   └── {Enums,Constants,Common}
│   ├── market.DataAccess
│   │   └── {Context,Configurations,Repositories,Services,UnitOfWork,Queries,Migrations,Extensions,DependencyInjection}
│   └── market.Utilities
│       └── {Email,SMS,FileManagement,Payment,Logging,Helpers,Extensions,Common}
├── tests
│   ├── market.UnitTests/{Services,Repositories,Common}
│   └── market.IntegrationTests/{API,MVC,Database,Fixtures}
├── docs
└── docker/{docker-compose.yml,postgres}
```

Repository root also contains `Directory.Build.props`, `Directory.Packages.props`, `.editorconfig`, `.gitignore`, `global.json`, and `README.md`.

Future feature folders are intentionally empty. The only implemented types support generic API pagination/responses, exception handling, EF Core setup, and unit of work. No marketplace business models, endpoints, or tables exist.
