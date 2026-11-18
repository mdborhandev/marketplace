using Microsoft.AspNetCore.Identity;
using market.Core.Interfaces.Services;
using market.Core.Models.Identity;

namespace market.DataAccess.Services.Implementations;

public sealed class PasswordHasher : IPasswordHasher
{
    private readonly Microsoft.AspNetCore.Identity.PasswordHasher<User> _inner = new();

    public string HashPassword(User user, string password)
        => _inner.HashPassword(user, password);

    public bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword)
        => _inner.VerifyHashedPassword(user, hashedPassword, providedPassword) != PasswordVerificationResult.Failed;
}