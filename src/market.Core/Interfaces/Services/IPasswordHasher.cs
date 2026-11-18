using market.Core.Models.Identity;

namespace market.Core.Interfaces.Services;

public interface IPasswordHasher
{
    string HashPassword(User user, string password);
    bool VerifyHashedPassword(User user, string hashedPassword, string providedPassword);
}