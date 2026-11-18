using market.Core.Models.Identity;

namespace market.Core.Interfaces.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default);
}