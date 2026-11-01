using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PoshakGhor.Core.Interfaces.Infrastructure;
using PoshakGhor.DataAccess.Context;
using PoshakGhor.DataAccess.UnitOfWork;

namespace PoshakGhor.DataAccess.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPoshakGhorDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        if (!string.IsNullOrWhiteSpace(connectionString))
            services.AddDbContext<PoshakGhorDbContext>(options => options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(PoshakGhorDbContext).Assembly.FullName)));
        else
            services.AddDbContext<PoshakGhorDbContext>(options => options.UseNpgsql("Host=localhost;Database=poshakghor;Username=poshakghor;Password=local-development-only"));
        services.AddScoped<IUnitOfWork, PoshakGhor.DataAccess.UnitOfWork.UnitOfWork>();
        return services;
    }
}
