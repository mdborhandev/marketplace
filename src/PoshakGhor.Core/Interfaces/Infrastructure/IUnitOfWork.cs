namespace PoshakGhor.Core.Interfaces.Infrastructure;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
