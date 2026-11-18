using Microsoft.EntityFrameworkCore;
using market.Core.Interfaces.Repositories;
using market.Core.Models.Identity;
using market.DataAccess.Context;
using market.DataAccess.Repositories.Base;

namespace market.DataAccess.Repositories.Implementations;

public sealed class EmailVerificationTokenRepository(marketDbContext context) : Repository<EmailVerificationToken>(context), IEmailVerificationTokenRepository
{
    public async Task<EmailVerificationToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default)
        => await Context.EmailVerificationTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);
}