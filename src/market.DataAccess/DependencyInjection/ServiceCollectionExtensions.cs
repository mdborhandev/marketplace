using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using market.Core.Interfaces.Infrastructure;
using market.DataAccess.Context;
using market.Core.Interfaces.Repositories;
using market.Core.Interfaces.Services;
using market.DataAccess.Repositories.Base;
using market.DataAccess.Repositories.Implementations;
using market.DataAccess.Services.Implementations;
using market.DataAccess.UnitOfWork;

namespace market.DataAccess.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddmarketDataAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("PostgreSQL");
        if (!string.IsNullOrWhiteSpace(connectionString))
            services.AddDbContext<marketDbContext>(options => options.UseNpgsql(connectionString, npgsql => npgsql.MigrationsAssembly(typeof(marketDbContext).Assembly.FullName)));
        else
            services.AddDbContext<marketDbContext>(options => options.UseNpgsql("Host=localhost;Database=market;Username=market;Password=local-development-only"));
        services.AddScoped<IUnitOfWork, market.DataAccess.UnitOfWork.UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        return services;
    }
}
