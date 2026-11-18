using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using market.Core.Interfaces.Repositories;
using market.DataAccess.Context;

namespace market.DataAccess.Repositories.Base;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly marketDbContext Context;
    protected readonly DbSet<TEntity> DbSet;

    public Repository(marketDbContext context)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync(new object[] { id }, cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().ToListAsync(cancellationToken);

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public virtual async Task<TEntity?> FirstOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public virtual async Task<bool> AnyAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(predicate, cancellationToken);

    public virtual async Task<int> CountAsync(
        Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        => predicate is null
            ? await DbSet.CountAsync(cancellationToken)
            : await DbSet.CountAsync(predicate, cancellationToken);

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await DbSet.AddRangeAsync(entities, cancellationToken);

    public virtual void Update(TEntity entity)
        => DbSet.Update(entity);

    public virtual void Remove(TEntity entity)
        => DbSet.Remove(entity);

    public virtual void RemoveRange(IEnumerable<TEntity> entities)
        => DbSet.RemoveRange(entities);
}