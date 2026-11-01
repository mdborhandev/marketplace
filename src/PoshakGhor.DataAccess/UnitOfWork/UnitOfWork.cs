using PoshakGhor.Core.Interfaces.Infrastructure;
using PoshakGhor.DataAccess.Context;

namespace PoshakGhor.DataAccess.UnitOfWork;

public sealed class UnitOfWork(PoshakGhorDbContext dbContext) : IUnitOfWork
{
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => dbContext.SaveChangesAsync(cancellationToken);
}
