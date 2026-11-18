using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using market.Core.Common;
using market.Core.Enums;
using market.Core.Interfaces.Infrastructure;
using market.Core.Interfaces.Repositories;
using market.Core.Interfaces.Services;
using market.Core.Models.Identity;
using market.DataAccess.Context;

namespace market.DataAccess.Services.Implementations;

public sealed class AuthService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IEmailVerificationTokenRepository emailVerificationTokenRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService,
    IUnitOfWork unitOfWork,
    IConfiguration configuration,
    marketDbContext dbContext) : IAuthService
{
    private readonly bool _requireConfirmedEmail =
        configuration.GetValue("Authentication:Identity:RequireConfirmedEmail", true);

    public async Task<AuthResult> RegisterAsync(string fullName, string? storeName, string email, string password, string role, ClientContext client, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(email);
        var existing = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (existing is not null)
            return AuthResult.Failure(AuthErrorCode.EmailAlreadyRegistered, "Email is already registered.");

        var requestedRole = string.Equals(role, "Vendor", StringComparison.OrdinalIgnoreCase) ? "Vendor" : "Customer";
        var roleEntity = await roleRepository.GetByNameAsync(requestedRole.ToUpperInvariant(), cancellationToken);
        if (roleEntity is null)
        {
            roleEntity = new Role
            {
                Id = Guid.NewGuid(),
                Name = requestedRole,
                NormalizedName = requestedRole.ToUpperInvariant(),
                IsSystemRole = true,
                CreatedAtUtc = DateTimeOffset.UtcNow
            };
            await roleRepository.AddAsync(roleEntity, cancellationToken);
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim(),
            NormalizedEmail = normalizedEmail,
            PasswordHash = string.Empty,
            FullName = fullName.Trim(),
            Status = UserStatus.PendingEmailVerification,
            SecurityStamp = RandomNumberGenerator.GetBytes(32),
            ConcurrencyStamp = RandomNumberGenerator.GetBytes(32),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            LockoutEnabled = true
        };
        user.PasswordHash = passwordHasher.HashPassword(user, password);

        await userRepository.AddAsync(user, cancellationToken);
        await dbContext.UserRoles.AddAsync(new UserRole { UserId = user.Id, RoleId = roleEntity.Id }, cancellationToken);

        var (rawToken, tokenHash, expires) = CreateSecureToken(TimeSpan.FromHours(24));
        await emailVerificationTokenRepository.AddAsync(new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ExpiresAtUtc = expires,
            CreatedByIp = client.IpAddress
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResult
        {
            Succeeded = true,
            User = user,
            Roles = [requestedRole],
            Message = "Registration successful. Please verify your email."
        };
    }

    public async Task<AuthResult> LoginAsync(string email, string password, bool rememberMe, ClientContext client, CancellationToken cancellationToken)
    {
        var normalizedEmail = NormalizeEmail(email);
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null)
        {
            await WriteAuditAsync(null, email, false, "InvalidCredentials", client, cancellationToken);
            return AuthResult.Failure(AuthErrorCode.InvalidCredentials, "Invalid email or password.");
        }

        if (user.LockoutEnabled && user.LockoutEndUtc.HasValue && user.LockoutEndUtc.Value > DateTimeOffset.UtcNow)
        {
            await WriteAuditAsync(user.Id, email, false, "AccountLocked", client, cancellationToken);
            return AuthResult.Failure(AuthErrorCode.AccountLocked, "Account is locked.");
        }

        if (user.Status is UserStatus.Suspended or UserStatus.Deactivated)
        {
            await WriteAuditAsync(user.Id, email, false, "AccountSuspended", client, cancellationToken);
            return AuthResult.Failure(AuthErrorCode.AccountSuspended, "Account is suspended.");
        }

        if (_requireConfirmedEmail && !user.EmailConfirmed)
        {
            await WriteAuditAsync(user.Id, email, false, "EmailNotVerified", client, cancellationToken);
            return AuthResult.Failure(AuthErrorCode.EmailNotVerified, "Email is not verified.");
        }

        if (!passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password))
        {
            user.AccessFailedCount++;
            if (user.LockoutEnabled && user.AccessFailedCount >= 5)
                user.LockoutEndUtc = DateTimeOffset.UtcNow.AddMinutes(15);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            await WriteAuditAsync(user.Id, email, false, "InvalidCredentials", client, cancellationToken);
            return AuthResult.Failure(AuthErrorCode.InvalidCredentials, "Invalid email or password.");
        }

        user.AccessFailedCount = 0;
        user.LockoutEndUtc = null;
        user.LastLoginAtUtc = DateTimeOffset.UtcNow;

        var roles = user.UserRoles.Select(x => x.Role.Name).ToList();
        var lifetimeDays = rememberMe ? 30 : 7;
        var (rawRefresh, refreshHash, refreshExpires) = tokenService.CreateRefreshToken(lifetimeDays);
        await refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = refreshHash,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ExpiresAtUtc = refreshExpires,
            CreatedByIp = client.IpAddress,
            UserAgent = client.UserAgent
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        await WriteAuditAsync(user.Id, email, true, null, client, cancellationToken);

        var accessToken = tokenService.CreateAccessToken(user, roles);
        return new AuthResult
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = rawRefresh,
            ExpiresIn = 900,
            User = user,
            Roles = roles
        };
    }

    public async Task<AuthResult> RefreshAsync(string refreshToken, ClientContext client, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return AuthResult.Failure(AuthErrorCode.RefreshTokenInvalid, "Refresh token is required.");

        var hash = tokenService.HashToken(refreshToken);
        var stored = await refreshTokenRepository.GetByHashAsync(hash, cancellationToken);
        if (stored is null)
            return AuthResult.Failure(AuthErrorCode.RefreshTokenInvalid, "Refresh token is invalid.");

        if (stored.RevokedAtUtc.HasValue)
        {
            foreach (var token in await refreshTokenRepository.GetActiveByUserAsync(stored.UserId, cancellationToken))
            {
                token.RevokedAtUtc = DateTimeOffset.UtcNow;
                token.RevokedByIp = client.IpAddress;
                refreshTokenRepository.Update(token);
            }
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return AuthResult.Failure(AuthErrorCode.RefreshTokenReused, "Refresh token reuse detected.");
        }

        if (stored.ExpiresAtUtc <= DateTimeOffset.UtcNow)
            return AuthResult.Failure(AuthErrorCode.TokenExpired, "Refresh token is expired.");

        if (stored.User is null || stored.User.Status is UserStatus.Suspended or UserStatus.Deactivated)
            return AuthResult.Failure(AuthErrorCode.AccountSuspended, "Account is unavailable.");

        stored.RevokedAtUtc = DateTimeOffset.UtcNow;
        stored.RevokedByIp = client.IpAddress;

        var roles = stored.User.UserRoles.Select(x => x.Role.Name).ToList();
        var (rawRefresh, refreshHash, refreshExpires) = tokenService.CreateRefreshToken(7);
        await refreshTokenRepository.AddAsync(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = stored.User.Id,
            TokenHash = refreshHash,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ExpiresAtUtc = refreshExpires,
            CreatedByIp = client.IpAddress,
            UserAgent = client.UserAgent
        }, cancellationToken);

        stored.ReplacedByTokenHash = refreshHash;
        refreshTokenRepository.Update(stored);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = tokenService.CreateAccessToken(stored.User, roles);
        return new AuthResult
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = rawRefresh,
            ExpiresIn = 900,
            User = stored.User,
            Roles = roles
        };
    }

    public async Task RevokeAsync(Guid userId, string? refreshToken, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(refreshToken))
        {
            var stored = await refreshTokenRepository.GetByHashAsync(tokenService.HashToken(refreshToken), cancellationToken);
            if (stored is not null && stored.UserId == userId)
            {
                stored.RevokedAtUtc = DateTimeOffset.UtcNow;
                refreshTokenRepository.Update(stored);
            }
        }
        else
        {
            foreach (var token in await refreshTokenRepository.GetActiveByUserAsync(userId, cancellationToken))
            {
                token.RevokedAtUtc = DateTimeOffset.UtcNow;
                refreshTokenRepository.Update(token);
            }
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ForgotPasswordAsync(string email, ClientContext client, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(NormalizeEmail(email), cancellationToken);
        if (user is null || user.Status is UserStatus.Suspended or UserStatus.Deactivated)
            return;

        var (rawToken, tokenHash, expires) = CreateSecureToken(TimeSpan.FromMinutes(30));
        await passwordResetTokenRepository.AddAsync(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ExpiresAtUtc = expires,
            RequestedByIp = client.IpAddress
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, ClientContext client, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(NormalizeEmail(email), cancellationToken);
        if (user is null)
            return false;

        var stored = await passwordResetTokenRepository.GetByHashAsync(tokenService.HashToken(token), cancellationToken);
        if (stored is null || stored.UserId != user.Id || stored.UsedAtUtc.HasValue || stored.ExpiresAtUtc <= DateTimeOffset.UtcNow)
            return false;

        user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
        user.SecurityStamp = RandomNumberGenerator.GetBytes(32);
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;
        userRepository.Update(user);

        stored.UsedAtUtc = DateTimeOffset.UtcNow;
        passwordResetTokenRepository.Update(stored);

        foreach (var refreshToken in await refreshTokenRepository.GetActiveByUserAsync(user.Id, cancellationToken))
        {
            refreshToken.RevokedAtUtc = DateTimeOffset.UtcNow;
            refreshToken.RevokedByIp = client.IpAddress;
            refreshTokenRepository.Update(refreshToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> VerifyEmailAsync(string email, string token, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(NormalizeEmail(email), cancellationToken);
        if (user is null)
            return false;

        var stored = await emailVerificationTokenRepository.GetByHashAsync(tokenService.HashToken(token), cancellationToken);
        if (stored is null || stored.UserId != user.Id || stored.UsedAtUtc.HasValue || stored.ExpiresAtUtc <= DateTimeOffset.UtcNow)
            return false;

        user.EmailConfirmed = true;
        if (user.Status == UserStatus.PendingEmailVerification)
            user.Status = UserStatus.Active;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;
        userRepository.Update(user);

        stored.UsedAtUtc = DateTimeOffset.UtcNow;
        emailVerificationTokenRepository.Update(stored);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task ResendVerificationAsync(string email, ClientContext client, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(NormalizeEmail(email), cancellationToken);
        if (user is null || user.EmailConfirmed)
            return;

        var (rawToken, tokenHash, expires) = CreateSecureToken(TimeSpan.FromHours(24));
        await emailVerificationTokenRepository.AddAsync(new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = tokenHash,
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ExpiresAtUtc = expires,
            CreatedByIp = client.IpAddress
        }, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetWithRolesAsync(userId, cancellationToken);
        if (user is null)
            return false;

        if (!passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword))
            return false;

        user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
        user.SecurityStamp = RandomNumberGenerator.GetBytes(32);
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;
        userRepository.Update(user);

        foreach (var refreshToken in await refreshTokenRepository.GetActiveByUserAsync(user.Id, cancellationToken))
        {
            refreshToken.RevokedAtUtc = DateTimeOffset.UtcNow;
            refreshTokenRepository.Update(refreshToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<User?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
        => await userRepository.GetWithRolesAsync(userId, cancellationToken);

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static (string RawToken, string TokenHash, DateTimeOffset ExpiresAtUtc) CreateSecureToken(TimeSpan lifetime)
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        var raw = Convert.ToBase64String(bytes);
        var hash = Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(raw))).ToLowerInvariant();
        return (raw, hash, DateTimeOffset.UtcNow.Add(lifetime));
    }

    private async Task WriteAuditAsync(Guid? userId, string email, bool succeeded, string? failureReason, ClientContext client, CancellationToken cancellationToken)
    {
        await dbContext.LoginAudits.AddAsync(new LoginAudit
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Email = email.Trim(),
            Succeeded = succeeded,
            FailureReason = failureReason,
            IpAddress = client.IpAddress,
            UserAgent = client.UserAgent,
            CreatedAtUtc = DateTimeOffset.UtcNow
        }, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}