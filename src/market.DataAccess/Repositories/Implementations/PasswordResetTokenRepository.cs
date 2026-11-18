using Microsoft.EntityFrameworkCore;
using market.Core.Interfaces.Repositories;
using market.Core.Models.Identity;
using market.DataAccess.Context;
using market.DataAccess.Repositories.Base;

namespace market.DataAccess.Repositories.Implementations;

public sealed class PasswordResetTokenRepository(marketDbContext context) : Repository<PasswordResetToken>(context), IPasswordResetTokenRepository
{
    public async Task<PasswordResetToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => await Context.PasswordResetTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
}