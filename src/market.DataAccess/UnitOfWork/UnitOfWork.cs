using market.Core.Interfaces.Infrastructure;
using market.DataAccess.Context;

namespace market.DataAccess.UnitOfWork;

public sealed class UnitOfWork(marketDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
