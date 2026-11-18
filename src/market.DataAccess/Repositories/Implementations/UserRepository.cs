using Microsoft.EntityFrameworkCore;
using market.Core.Interfaces.Repositories;
using market.Core.Models.Identity;
using market.DataAccess.Context;
using market.DataAccess.Repositories.Base;

namespace market.DataAccess.Repositories.Implementations;

public sealed class UserRepository(marketDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        => await Context.Users
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.NormalizedEmail == normalizedEmail, cancellationToken);

    public async Task<User?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default)
        => await Context.Users
            .Include(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.Id == userId, cancellationToken);
}