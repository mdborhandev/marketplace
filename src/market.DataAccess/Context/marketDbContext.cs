using Microsoft.EntityFrameworkCore;

namespace market.DataAccess.Context;

public sealed class marketDbContext(DbContextOptions<marketDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(marketDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
