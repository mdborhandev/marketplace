using Microsoft.EntityFrameworkCore;

namespace PoshakGhor.DataAccess.Context;

public sealed class PoshakGhorDbContext(DbContextOptions<PoshakGhorDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PoshakGhorDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
