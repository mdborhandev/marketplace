using market.Core.Models.Identity;

namespace market.Core.Interfaces.Repositories;

public interface IEmailVerificationTokenRepository : IRepository<EmailVerificationToken>
{
    Task<EmailVerificationToken?> GetByHashAsync(string tokenHash, CancellationToken cancellationToken = default);
}