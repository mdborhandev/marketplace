using market.Core.Models.Identity;

namespace market.Core.Interfaces.Services;

public interface ITokenService
{
    string CreateAccessToken(User user, IReadOnlyList<string> roles);
    (string RawToken, string TokenHash, DateTimeOffset ExpiresAtUtc) CreateRefreshToken(int lifetimeDays);
    string HashToken(string rawToken);
    bool VerifyTokenHash(string rawToken, string storedHash);
}