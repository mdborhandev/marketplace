using Microsoft.EntityFrameworkCore;
using market.Core.Interfaces.Repositories;
using market.Core.Models.Identity;
using market.DataAccess.Context;
using market.DataAccess.Repositories.Base;

namespace market.DataAccess.Repositories.Implementations;

public sealed class RefreshTokenRepository(marketDbContext context) : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => await Context.RefreshTokens
            .Include(x => x.User).ThenInclude(x => x.UserRoles).ThenInclude(x => x.Role)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

    public async Task<IReadOnlyList<RefreshToken>> GetActiveByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        => await Context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAtUtc == null && x.ExpiresAtUtc > DateTimeOffset.UtcNow)
            .ToListAsync(cancellationToken);
}