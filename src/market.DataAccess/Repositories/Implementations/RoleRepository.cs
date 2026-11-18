using Microsoft.EntityFrameworkCore;
using market.Core.Interfaces.Repositories;
using market.Core.Models.Identity;
using market.DataAccess.Context;
using market.DataAccess.Repositories.Base;

namespace market.DataAccess.Repositories.Implementations;

public sealed class RoleRepository(marketDbContext context) : Repository<Role>(context), IRoleRepository
{
    public async Task<Role?> GetByNameAsync(string normalizedName, CancellationToken cancellationToken = default)
        => await Context.Roles.FirstOrDefaultAsync(x => x.NormalizedName == normalizedName, cancellationToken);
}