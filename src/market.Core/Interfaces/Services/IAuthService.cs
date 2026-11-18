using market.Core.Common;
using market.Core.Models.Identity;

namespace market.Core.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(string fullName, string? storeName, string email, string password, string role, ClientContext client, CancellationToken cancellationToken);
    Task<AuthResult> LoginAsync(string email, string password, bool rememberMe, ClientContext client, CancellationToken cancellationToken);
    Task<AuthResult> RefreshAsync(string refreshToken, ClientContext client, CancellationToken cancellationToken);
    Task RevokeAsync(Guid userId, string? refreshToken, CancellationToken cancellationToken);
    Task ForgotPasswordAsync(string email, ClientContext client, CancellationToken cancellationToken);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword, ClientContext client, CancellationToken cancellationToken);
    Task<bool> VerifyEmailAsync(string email, string token, CancellationToken cancellationToken);
    Task ResendVerificationAsync(string email, ClientContext client, CancellationToken cancellationToken);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken);
    Task<User?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
}