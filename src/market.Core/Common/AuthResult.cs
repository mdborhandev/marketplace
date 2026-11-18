using market.Core.Enums;
using market.Core.Models.Identity;

namespace market.Core.Common;

public sealed class AuthResult
{
    public bool Succeeded { get; init; }
    public AuthErrorCode? ErrorCode { get; init; }
    public string? Message { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public int ExpiresIn { get; init; }
    public User? User { get; init; }
    public IReadOnlyList<string> Roles { get; init; } = [];

    public static AuthResult Success(User user, IReadOnlyList<string> roles, string? accessToken = null, string? refreshToken = null, int expiresIn = 0, string? message = null) => new()
    {
        Succeeded = true,
        User = user,
        Roles = roles,
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        ExpiresIn = expiresIn,
        Message = message
    };

    public static AuthResult Failure(AuthErrorCode errorCode, string? message = null) => new()
    {
        Succeeded = false,
        ErrorCode = errorCode,
        Message = message
    };
}