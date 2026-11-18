# End-to-End Authentication Plan: Login, Registration, and Related Flows

## 1. Purpose and scope

This plan defines the complete authentication and account lifecycle for the `market` marketplace, covering:

- Merchant (vendor) and customer registration
- Email/password login for browser (MVC) and mobile/API clients
- Logout and session/token revocation
- Email verification
- Forgot password and reset password
- Change password
- Refresh token rotation for API clients
- OTP verification (email/SMS) for registration, login, and password reset
- Optional two-factor authentication (TOTP + recovery codes)
- Role-based authorization and account status management
- Login auditing and basic abuse protection

The plan is documentation only. It is intended to be implemented in later feature work without breaking the existing layered architecture, routing, or authentication foundation.

## 2. Current state and gaps

Implemented today:

- Named cookie scheme `market.Cookie` and JWT bearer scheme `market.Bearer`
- `AuthController` returns static `Login` and `Register` views
- Static forms without model binding or POST handlers
- `marketDbContext` has no identity or token entities
- No user, role, token, OTP, or audit storage
- No email/SMS sender implementation

Not implemented and explicitly deferred by the architecture spec:

- Login and registration
- OTP verification
- Token generation
- Refresh-token storage/workflows
- User identity business features

This plan converts those deferred items into a structured implementation roadmap.

## 3. Architecture constraints to preserve

- `market.Core` remains framework-independent.
- `market.DataAccess` may reference framework packages and implements service/repository abstractions from Core.
- `market.Application` composes MVC and API behavior and contains controllers, ViewModels, routing, and DI registration.
- MVC and API controllers use the same application service abstractions; business logic is not duplicated.
- All I/O is async and accepts `CancellationToken`.
- Secrets stay outside source control.
- No business logic is placed in controllers.

## 4. Recommended identity approach

### Recommendation

Use **custom domain identity entities in `market.Core`** instead of ASP.NET Core Identity entities.

Reasons:

- Preserves framework independence of `market.Core`
- Gives full control over marketplace-specific fields, statuses, and audit data
- Keeps role, token, and OTP models explicit and testable
- Avoids hidden Identity schema and middleware behavior
- Still allows reuse of proven password-hashing algorithms in `market.DataAccess`

Security-sensitive operations are implemented in `market.DataAccess` and `market.Utilities`, not in controllers.

### Alternative

ASP.NET Core Identity can be considered later if the team wants built-in lockout, 2FA, and token providers. If adopted, it should be isolated to `market.DataAccess` and `market.Application`, with Core still exposing framework-independent abstractions. The custom approach below remains the default recommendation.

## 5. Layer and request flow

```text
MVC View / API Client
        |
        v
MVC Controller / API Controller
        |
        v
Application Service Interface (market.Core)
        |
        v
Service Implementation (market.DataAccess)
        |
        +--> Password hasher (market.DataAccess / Utilities)
        +--> Token service (market.DataAccess)
        +--> Repositories (market.DataAccess)
        |
        v
EF Core DbContext / PostgreSQL
```

Shared services:

- `IAuthService`
- `ITokenService`
- `IOtpService`
- `ICurrentUserService`
- `IEmailSender`
- `ISmsSender`

## 6. Model changes required

### 6.1 New Core model folder

```text
src/market.Core/Models/Identity/
├── User.cs
├── Role.cs
├── UserRole.cs
├── RefreshToken.cs
├── EmailVerificationToken.cs
├── PasswordResetToken.cs
├── OtpCode.cs
└── LoginAudit.cs
```

### 6.2 User entity

```csharp
public sealed class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string NormalizedEmail { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    public DateTimeOffset? LockoutEndUtc { get; set; }
    public bool LockoutEnabled { get; set; } = true;
    public UserStatus Status { get; set; } = UserStatus.PendingEmailVerification;
    public byte[] SecurityStamp { get; set; } = [];
    public byte[] ConcurrencyStamp { get; set; } = [];
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset? UpdatedAtUtc { get; set; }
    public DateTimeOffset? LastLoginAtUtc { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = [];
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = [];
    public ICollection<OtpCode> OtpCodes { get; set; } = [];
}
```

Notes:

- `Email` is the login identifier.
- `Status` controls whether login is allowed.
- `SecurityStamp` is rotated after password change/reset and email change.
- `ConcurrencyStamp` supports optimistic concurrency.

### 6.3 Role entity

```csharp
public sealed class Role
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }

    public ICollection<UserRole> UserRoles { get; set; } = [];
}
```

System roles:

- `SuperAdmin`
- `Admin`
- `Vendor`
- `Customer`
- `Support`

### 6.4 UserRole join entity

```csharp
public sealed class UserRole
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
```

Composite primary key: `(UserId, RoleId)`.

### 6.5 RefreshToken entity

```csharp
public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public string? JwtId { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? RevokedAtUtc { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public string? UserAgent { get; set; }

    public User User { get; set; } = null!;
}
```

Only a hash of the refresh token is stored. The raw token is returned once to the client.

### 6.6 EmailVerificationToken entity

```csharp
public sealed class EmailVerificationToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? UsedAtUtc { get; set; }
    public string? CreatedByIp { get; set; }

    public User User { get; set; } = null!;
}
```

### 6.7 PasswordResetToken entity

```csharp
public sealed class PasswordResetToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? UsedAtUtc { get; set; }
    public string? RequestedByIp { get; set; }

    public User User { get; set; } = null!;
}
```

### 6.8 OtpCode entity

```csharp
public sealed class OtpCode
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Target { get; set; } = string.Empty; // email or phone
    public OtpPurpose Purpose { get; set; }
    public OtpChannel Channel { get; set; }
    public string CodeHash { get; set; } = string.Empty;
    public int AttemptCount { get; set; }
    public int MaxAttempts { get; set; } = 5;
    public DateTimeOffset CreatedAtUtc { get; set; }
    public DateTimeOffset ExpiresAtUtc { get; set; }
    public DateTimeOffset? UsedAtUtc { get; set; }
    public string? CreatedByIp { get; set; }

    public User? User { get; set; }
}
```

### 6.9 LoginAudit entity

```csharp
public sealed class LoginAudit
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool Succeeded { get; set; }
    public string? FailureReason { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}
```

### 6.10 Enums

Create in `market.Core/Enums`:

- `UserStatus`: `PendingEmailVerification`, `Active`, `Suspended`, `Deactivated`
- `OtpPurpose`: `Registration`, `Login`, `TwoFactor`, `PasswordReset`, `PhoneVerification`
- `OtpChannel`: `Email`, `Sms`

### 6.11 DbContext changes

Add to `marketDbContext`:

```csharp
public DbSet<User> Users => Set<User>();
public DbSet<Role> Roles => Set<Role>();
public DbSet<UserRole> UserRoles => Set<UserRole>();
public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
public DbSet<OtpCode> OtpCodes => Set<OtpCode>();
public DbSet<LoginAudit> LoginAudits => Set<LoginAudit>();
```

Entity configurations should be added under:

```text
src/market.DataAccess/Configurations/Identity/
├── UserConfiguration.cs
├── RoleConfiguration.cs
├── UserRoleConfiguration.cs
├── RefreshTokenConfiguration.cs
├── EmailVerificationTokenConfiguration.cs
├── PasswordResetTokenConfiguration.cs
├── OtpCodeConfiguration.cs
└── LoginAuditConfiguration.cs
```

Important indexes:

- `User.NormalizedEmail` unique
- `Role.NormalizedName` unique
- `RefreshToken.TokenHash` unique
- `RefreshToken.UserId`
- `RefreshToken.ExpiresAtUtc`
- `EmailVerificationToken.TokenHash` unique
- `PasswordResetToken.TokenHash` unique
- `OtpCode.Target`, `Purpose`, `ExpiresAtUtc`
- `LoginAudit.UserId`, `CreatedAtUtc`, `IpAddress`

### 6.12 Migration plan

1. Add entities and configurations.
2. Create migration: `InitialIdentitySchema`.
3. Review generated SQL.
4. Apply in development.
5. Add idempotent role seeding through a startup seeder, not through migration data if avoidable.
6. Add a separate migration only if schema changes.

No marketplace business tables are included in this auth migration.

## 7. DTO and ViewModel changes

### 7.1 API request DTOs

Create in `market.Core/DTOs/Requests/Auth/`:

- `RegisterRequest`
- `LoginRequest`
- `RefreshTokenRequest`
- `ForgotPasswordRequest`
- `ResetPasswordRequest`
- `VerifyEmailRequest`
- `ResendVerificationRequest`
- `ChangePasswordRequest`
- `RequestOtpRequest`
- `VerifyOtpRequest`

Suggested fields:

`RegisterRequest`

```csharp
public sealed record RegisterRequest(
    string FullName,
    string? StoreName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? Role, // Customer or Vendor only
    bool AcceptTerms,
    string? CaptchaToken
);
```

`LoginRequest`

```csharp
public sealed record LoginRequest(
    string Email,
    string Password,
    bool RememberMe,
    string? TwoFactorCode
);
```

`RefreshTokenRequest`

```csharp
public sealed record RefreshTokenRequest(string RefreshToken);
```

`ForgotPasswordRequest`

```csharp
public sealed record ForgotPasswordRequest(string Email);
```

`ResetPasswordRequest`

```csharp
public sealed record ResetPasswordRequest(
    string Email,
    string Token,
    string NewPassword,
    string ConfirmPassword
);
```

`VerifyEmailRequest`

```csharp
public sealed record VerifyEmailRequest(string Email, string Token);
```

`ChangePasswordRequest`

```csharp
public sealed record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword,
    string ConfirmPassword
);
```

`RequestOtpRequest`

```csharp
public sealed record RequestOtpRequest(
    string Target,
    OtpPurpose Purpose,
    OtpChannel Channel
);
```

`VerifyOtpRequest`

```csharp
public sealed record VerifyOtpRequest(
    Guid ChallengeId,
    string Code
);
```

### 7.2 API response DTOs

Create in `market.Core/DTOs/Responses/Auth/`:

- `AuthResponse`
- `UserSummaryResponse`
- `OtpChallengeResponse`

```csharp
public sealed record AuthResponse(
    string AccessToken,
    string TokenType,
    int ExpiresIn,
    string? RefreshToken,
    UserSummaryResponse User
);

public sealed record UserSummaryResponse(
    Guid Id,
    string Email,
    string? FullName,
    IReadOnlyList<string> Roles,
    bool EmailConfirmed
);

public sealed record OtpChallengeResponse(
    Guid ChallengeId,
    DateTimeOffset ExpiresAtUtc
);
```

### 7.3 MVC ViewModels

Create in `src/market.Application/ViewModels/Auth/`:

- `LoginViewModel`
- `RegisterViewModel`
- `ForgotPasswordViewModel`
- `ResetPasswordViewModel`
- `VerifyEmailViewModel`
- `ChangePasswordViewModel`
- `OtpVerificationViewModel`

These mirror API DTOs but use data annotations for unobtrusive validation.

## 8. Service and repository contracts

### 8.1 Core interfaces

Create in `market.Core/Interfaces/Services/`:

- `IAuthService`
- `ITokenService`
- `IOtpService`
- `IPasswordHasher` (framework-independent abstraction)
- `ICurrentUserService`

Create in `market.Core/Interfaces/Repositories/`:

- `IUserRepository`
- `IRoleRepository`
- `IRefreshTokenRepository`
- `IEmailVerificationTokenRepository`
- `IPasswordResetTokenRepository`
- `IOtpCodeRepository`
- `ILoginAuditRepository`

### 8.2 IAuthService

```csharp
public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequest request, ClientContext client, CancellationToken cancellationToken);
    Task<AuthResult> LoginAsync(LoginRequest request, ClientContext client, CancellationToken cancellationToken);
    Task<AuthResult> RefreshAsync(RefreshTokenRequest request, ClientContext client, CancellationToken cancellationToken);
    Task RevokeAsync(Guid userId, string? refreshToken, CancellationToken cancellationToken);
    Task ForgotPasswordAsync(ForgotPasswordRequest request, ClientContext client, CancellationToken cancellationToken);
    Task ResetPasswordAsync(ResetPasswordRequest request, ClientContext client, CancellationToken cancellationToken);
    Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken cancellationToken);
    Task ResendVerificationAsync(ResendVerificationRequest request, ClientContext client, CancellationToken cancellationToken);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken cancellationToken);
    Task<UserSummaryResponse?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken);
}
```

`AuthResult` should be a Core result type containing either success data or a typed failure reason, avoiding exceptions for expected auth failures.

### 8.3 ITokenService

```csharp
public interface ITokenService
{
    string CreateAccessToken(User user, IReadOnlyList<string> roles, IEnumerable<Claim> additionalClaims);
    (string RawToken, string TokenHash, DateTimeOffset ExpiresAtUtc) CreateRefreshToken(int lifetimeDays);
    bool VerifyTokenHash(string rawToken, string storedHash);
    string HashToken(string rawToken);
}
```

### 8.4 IOtpService

```csharp
public interface IOtpService
{
    Task<OtpChallengeResponse> GenerateAsync(RequestOtpRequest request, ClientContext client, CancellationToken cancellationToken);
    Task<bool> VerifyAsync(VerifyOtpRequest request, CancellationToken cancellationToken);
}
```

### 8.5 ICurrentUserService

```csharp
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    IReadOnlyList<string> Roles { get; }
}
```

### 8.6 ClientContext

Create in `market.Core/Common` or `Contracts`:

```csharp
public sealed record ClientContext(string? IpAddress, string? UserAgent);
```

## 9. Routing plan

### 9.1 MVC routes

Keep existing default and area routes. Add explicit auth routes in `AuthController` and `AccountController`.

MVC controller: `src/market.Application/Controllers/AuthController.cs`

| Method | Route | Action | Purpose |
|---|---|---|---|
| GET | `/login` | `Login` | Show login page |
| POST | `/login` | `Login` | Cookie sign-in |
| GET | `/register` | `Register` | Show registration page |
| POST | `/register` | `Register` | Create account |
| POST | `/logout` | `Logout` | Sign out cookie |
| GET | `/forgot-password` | `ForgotPassword` | Show request form |
| POST | `/forgot-password` | `ForgotPassword` | Send reset link |
| GET | `/reset-password` | `ResetPassword` | Show reset form |
| POST | `/reset-password` | `ResetPassword` | Apply new password |
| GET | `/verify-email` | `VerifyEmail` | Verify email token |
| GET | `/account` | `Index` | Current user profile |
| GET/POST | `/account/change-password` | `ChangePassword` | Change password |
| GET/POST | `/account/security` | `Security` | 2FA/OTP settings |

MVC controller: `src/market.Application/Controllers/AccountController.cs`

Authenticated account-management endpoints should require `[Authorize]`.

### 9.2 API routes

Create `src/market.Application/APIControllers/V1/AuthController.cs`.

Base route:

```csharp
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
```

| Method | Route | Action | Auth |
|---|---|---|---|
| POST | `/api/v1/auth/register` | `Register` | Anonymous |
| POST | `/api/v1/auth/login` | `Login` | Anonymous |
| POST | `/api/v1/auth/refresh` | `Refresh` | Anonymous (refresh token in body) |
| POST | `/api/v1/auth/revoke` | `Revoke` | Bearer |
| POST | `/api/v1/auth/forgot-password` | `ForgotPassword` | Anonymous |
| POST | `/api/v1/auth/reset-password` | `ResetPassword` | Anonymous |
| POST | `/api/v1/auth/verify-email` | `VerifyEmail` | Anonymous |
| POST | `/api/v1/auth/resend-verification` | `ResendVerification` | Anonymous |
| POST | `/api/v1/auth/change-password` | `ChangePassword` | Bearer |
| GET | `/api/v1/auth/me` | `Me` | Bearer |
| POST | `/api/v1/auth/otp/request` | `RequestOtp` | Anonymous or Bearer depending purpose |
| POST | `/api/v1/auth/otp/verify` | `VerifyOtp` | Anonymous or Bearer |

### 9.3 Route naming and redirects

- Use named routes: `Login`, `Register`, `ForgotPassword`, `ResetPassword`.
- After successful login, redirect to `returnUrl` if local; otherwise `/`.
- After registration, redirect to email verification notice page.
- After logout, redirect to `/login`.

## 10. Authentication scheme and authorization changes

### 10.1 Program.cs changes

- Keep cookie and JWT schemes.
- Register `IAuthService`, `ITokenService`, `IOtpService`, `ICurrentUserService`, repositories.
- Register `IPasswordHasher` implementation.
- Configure Identity options manually:
  - Password length and complexity
  - Lockout threshold and duration
  - User requirements
  - Sign-in requirements
- Add a stricter rate limiter named `auth`.
- Add authorization policies:
  - `CustomerOnly`
  - `VendorOnly`
  - `AdminOnly`
  - `SupportOrAdmin`

### 10.2 Cookie behavior

- MVC login uses `HttpContext.SignInAsync("market.Cookie", principal, authProperties)`.
- `IsPersistent` follows `RememberMe`.
- Sliding expiration remains enabled.
- Cookie remains HttpOnly, Secure, SameSite=Lax.

### 10.3 JWT behavior

- API login returns access token + refresh token.
- Access token lifetime: 15 minutes.
- Refresh token lifetime: 7 days, or 30 days when `RememberMe` is true.
- Include claims: `sub`, `email`, `name`, `role`, `jti`, `iat`.
- Validate issuer, audience, signing key, lifetime.

## 11. Security requirements

### 11.1 Passwords

- Minimum 8 characters
- Require uppercase, lowercase, digit, and symbol
- Reject common passwords through a small deny list
- Store only password hashes
- Rotate security stamp on password change/reset

### 11.2 Account protection

- Lockout after 5 failed attempts for 15 minutes
- Reset failed count on successful login
- Disable login for `Suspended` and `Deactivated` users
- Require verified email before login when configured
- Record every login attempt in `LoginAudit`

### 11.3 Tokens

- Refresh tokens are opaque, high-entropy, and hashed before storage
- Rotate refresh token on every refresh
- Detect reuse and revoke token family
- Single-use email verification and password reset tokens
- Short expiry:
  - Email verification: 24 hours
  - Password reset: 30 minutes
  - OTP: 10 minutes
  - Access token: 15 minutes

### 11.4 Abuse protection

- Apply `auth` rate limiter to anonymous auth endpoints
- Add resend cooldown for verification and OTP
- Add CAPTCHA hook for registration and forgot-password
- Avoid user enumeration in forgot-password responses
- Return generic errors for invalid credentials

### 11.5 Transport and browser security

- HTTPS only
- Secure cookies only
- Anti-forgery tokens on MVC POST forms
- No tokens in query strings except one-time verification/reset tokens
- Sanitize and validate `returnUrl`

## 12. Email and SMS integration

Create abstractions in `market.Utilities`:

- `IEmailSender`
- `ISmsSender`

Initial implementation can be a development/logging sender. Production providers are added later.

Required templates:

- Welcome/verification email
- Password reset email
- OTP email
- Password changed notification
- New login notification (optional)

## 13. Frontend and view changes

### 13.1 Existing views

Update:

- `Views/Auth/Login.cshtml`
- `Views/Auth/Register.cshtml`

Changes:

- Add `@model` directives
- Use `asp-action`, `asp-controller`, `asp-route-returnUrl`
- Add validation spans
- Add anti-forgery token
- Add links to forgot password and verification resend
- Preserve theme toggle behavior

### 13.2 New views

Create:

- `Views/Auth/ForgotPassword.cshtml`
- `Views/Auth/ResetPassword.cshtml`
- `Views/Auth/VerifyEmail.cshtml`
- `Views/Auth/VerificationSent.cshtml`
- `Views/Account/Index.cshtml`
- `Views/Account/ChangePassword.cshtml`
- `Views/Account/Security.cshtml`
- `Views/Shared/_AuthValidationScriptsPartial.cshtml` if needed

### 13.3 JavaScript

- Keep `auth-theme.js`
- Add optional client-side password strength indicator
- Do not place auth secrets in client code

## 14. Vendor registration and profile creation

Registration supports `Customer` and `Vendor` roles only. Public registration never creates `Admin`, `SuperAdmin`, or `Support`.

For `Vendor` registration:

1. Create user with `Vendor` role.
2. Set status based on email verification policy.
3. Create or queue creation of vendor profile/store data through the future Vendor module.
4. Store `StoreName` in the registration request and pass it to the vendor profile service.

Auth should not own vendor business data. It should emit an application event or call `IVendorProfileService` if available.

## 15. Database and seeding plan

### 15.1 Seeding

Create `IdentitySeeder` in `market.DataAccess` or `market.Application` startup:

- Seed system roles if missing
- Optionally seed one development admin user from configuration
- Never seed production users automatically
- Seeder must be idempotent

### 15.2 Configuration keys

```json
{
  "Authentication": {
    "Cookie": {
      "Name": "market.Auth"
    },
    "Jwt": {
      "Issuer": "",
      "Audience": "",
      "SigningKey": "",
      "AccessTokenMinutes": 15,
      "RefreshTokenDays": 7,
      "RefreshTokenRememberMeDays": 30
    },
    "Identity": {
      "RequireConfirmedEmail": true,
      "Password": {
        "RequiredLength": 8,
        "RequireUppercase": true,
        "RequireLowercase": true,
        "RequireDigit": true,
        "RequireNonAlphanumeric": true
      },
      "Lockout": {
        "MaxFailedAccessAttempts": 5,
        "DefaultLockoutMinutes": 15
      }
    },
    "Otp": {
      "Length": 6,
      "ExpiryMinutes": 10,
      "MaxAttempts": 5,
      "ResendCooldownSeconds": 60
    }
  }
}
```

Secrets are provided through environment variables or user secrets.

## 16. Testing plan

### Unit tests

- Registration validation and duplicate email handling
- Password hashing and verification
- Login success/failure
- Lockout behavior
- Token creation and hash verification
- Refresh rotation and reuse detection
- OTP generation, expiry, and attempt limits
- Password reset single-use behavior
- Role assignment restrictions

### Integration tests

- MVC login and register pages render
- MVC login POST sets auth cookie
- API register/login/refresh flow
- Protected endpoint returns 401/403 correctly
- Email verification flow
- Password reset flow
- Migration applies to PostgreSQL test database
- Rate limiter behavior on auth endpoints

### Security tests

- Expired access/refresh tokens rejected
- Revoked refresh token rejected
- Reused refresh token revokes family
- Invalid reset token rejected
- OTP brute-force attempts blocked
- Open redirect protection
- User enumeration resistance

## 17. Implementation phases

### Phase 1: Foundation

- Add model and enum files
- Add repository interfaces
- Add EF configurations
- Update DbContext
- Create and review migration
- Add password hasher implementation

### Phase 2: Core auth services

- Implement `AuthService`
- Implement `TokenService`
- Implement repositories
- Add unit tests for core logic

### Phase 3: MVC flows

- Update Login and Register views
- Add ForgotPassword, ResetPassword, VerifyEmail flows
- Add Account controller and views
- Wire cookie sign-in/sign-out
- Add anti-forgery and validation

### Phase 4: API flows

- Create V1 auth controller
- Add request/response DTOs
- Wire JWT and refresh tokens
- Add Swagger examples and auth metadata

### Phase 5: OTP and advanced security

- Implement OTP service
- Add OTP endpoints/views
- Add login audit
- Add rate limiter and CAPTCHA hooks
- Add two-factor setup (optional)

### Phase 6: Hardening and documentation

- Complete integration/security tests
- Update `docs/AUTHENTICATION.md`
- Update `docs/DATABASE_DESIGN.md`
- Update `docs/PROJECT_STRUCTURE.md`
- Run `dotnet format market.sln`

## 18. Files and folders to add

```text
src/market.Core/Models/Identity/
src/market.Core/Enums/UserStatus.cs
src/market.Core/Enums/OtpPurpose.cs
src/market.Core/Enums/OtpChannel.cs
src/market.Core/DTOs/Requests/Auth/
src/market.Core/DTOs/Responses/Auth/
src/market.Core/Interfaces/Services/IAuthService.cs
src/market.Core/Interfaces/Services/ITokenService.cs
src/market.Core/Interfaces/Services/IOtpService.cs
src/market.Core/Interfaces/Services/IPasswordHasher.cs
src/market.Core/Interfaces/Services/ICurrentUserService.cs
src/market.Core/Interfaces/Repositories/IUserRepository.cs
src/market.Core/Interfaces/Repositories/IRoleRepository.cs
src/market.Core/Interfaces/Repositories/IRefreshTokenRepository.cs
src/market.Core/Interfaces/Repositories/IEmailVerificationTokenRepository.cs
src/market.Core/Interfaces/Repositories/IPasswordResetTokenRepository.cs
src/market.Core/Interfaces/Repositories/IOtpCodeRepository.cs
src/market.Core/Interfaces/Repositories/ILoginAuditRepository.cs
src/market.Core/Common/ClientContext.cs

src/market.DataAccess/Configurations/Identity/
src/market.DataAccess/Repositories/Implementations/
src/market.DataAccess/Services/Implementations/AuthService.cs
src/market.DataAccess/Services/Implementations/TokenService.cs
src/market.DataAccess/Services/Implementations/OtpService.cs
src/market.DataAccess/Services/Implementations/PasswordHasher.cs
src/market.DataAccess/Seeding/IdentitySeeder.cs

src/market.Application/Controllers/AuthController.cs
src/market.Application/Controllers/AccountController.cs
src/market.Application/APIControllers/V1/AuthController.cs
src/market.Application/ViewModels/Auth/
src/market.Application/Views/Auth/
src/market.Application/Views/Account/
```

## 19. Risks and open decisions

- Whether `Vendor` registration should require admin approval before store activation
- Whether email verification should block login for customers as well as vendors
- Whether OTP is required for all logins or only high-risk actions
- Whether to support social login (Google/Apple/Facebook) in a later phase
- Whether to use SMS, email, or both for OTP delivery
- Whether refresh tokens should be stored in database or distributed cache
- Whether password reset should invalidate all existing sessions

These decisions should be finalized before Phase 3.

## 21. Detailed flow specifications

### 21.1 Registration flow

Preconditions:
- Email is not already registered.
- Requested role is `Customer` or `Vendor`.
- Terms are accepted.
- CAPTCHA passes when enabled.

Steps:
1. Normalize email.
2. Check uniqueness.
3. Validate password policy.
4. Create `User` with `PendingEmailVerification` status.
5. Assign requested role.
6. Hash password.
7. Generate email verification token.
8. Persist user, role assignment, and token in one transaction.
9. Queue verification email through outbox.
10. If role is `Vendor`, create or schedule vendor profile creation with `StoreName`.
11. Write audit entry.
12. Return success without exposing whether email existed.

Postconditions:
- User exists.
- Verification token exists and is unexpired.
- No session is created until email is verified when verification is required.

Failure modes:
- Duplicate email: return generic success to avoid enumeration, but do not create duplicate.
- Weak password: validation error.
- Email delivery failure: user remains unverified and can request resend.

### 21.2 Login flow (MVC)

Steps:
1. Validate request.
2. Load user by normalized email.
3. Check account status and lockout.
4. Verify password.
5. If 2FA/OTP is required, issue challenge and stop.
6. Update `LastLoginAtUtc` and reset failed count.
7. Create claims principal.
8. Sign in with `market.Cookie`.
9. Write login audit.
10. Redirect to safe `returnUrl` or home.

### 21.3 Login flow (API)

Same as MVC through step 6, then:
7. Generate access token and refresh token.
8. Persist hashed refresh token.
9. Return `AuthResponse`.
10. Write login audit.

### 21.4 Refresh flow

Steps:
1. Validate refresh token format.
2. Hash incoming token.
3. Load stored refresh token and user.
4. Reject if revoked, expired, or user inactive.
5. Detect reuse: if token already revoked, revoke entire token family.
6. Create new refresh token and revoke old one.
7. Generate new access token.
8. Persist changes in one transaction.
9. Return new tokens.

### 21.5 Logout flow

MVC:
1. Sign out cookie.
2. Optionally revoke all refresh tokens for the user.
3. Redirect to login.

API:
1. Authenticate bearer token.
2. Revoke supplied refresh token or all tokens.
3. Return 204.

### 21.6 Email verification flow

Steps:
1. Generate high-entropy token.
2. Hash and store with expiry.
3. Send email with raw token.
4. On verification request, hash incoming token and compare.
5. Reject if expired or used.
6. Mark user email confirmed.
7. Update status to `Active` when required.
8. Mark token used.
9. Write audit.

### 21.7 Forgot password flow

Steps:
1. Normalize email.
2. If user exists and active, generate reset token.
3. Store hashed token.
4. Send email.
5. Always return generic success.

### 21.8 Reset password flow

Steps:
1. Validate email, token, and new password.
2. Load user and latest valid reset token.
3. Verify token hash and expiry.
4. Update password hash.
5. Rotate security stamp.
6. Revoke all refresh tokens.
7. Mark reset token used.
8. Write audit.

### 21.9 Change password flow

Steps:
1. Authenticate user.
2. Verify current password.
3. Validate new password policy and ensure it differs.
4. Update password hash.
5. Rotate security stamp.
6. Revoke all other refresh tokens except current if supported.
7. Write audit.

### 21.10 OTP flow

Request:
1. Validate target and purpose.
2. Enforce resend cooldown.
3. Generate 6-digit code.
4. Hash and store with expiry.
5. Send via email or SMS.
6. Return challenge ID.

Verify:
1. Load challenge.
2. Reject if expired or used.
3. Increment attempt count.
4. Reject after max attempts.
5. Compare hash.
6. Mark used on success.
7. Return result.

## 22. Validation and error contract

### 22.1 Validation rules

- Email: required, valid format, max 256 chars, normalized.
- Password: required, policy enforced.
- ConfirmPassword: must equal Password.
- FullName: required, 2-100 chars.
- StoreName: required for Vendor, 2-100 chars.
- Role: must be Customer or Vendor for public registration.
- Token: required, non-empty.
- OTP code: required, 6 digits.
- CaptchaToken: required when CAPTCHA enabled.

### 22.2 Auth error codes

Create `market.Core/Enums/AuthErrorCode` or constants:

- `InvalidCredentials`
- `AccountLocked`
- `AccountSuspended`
- `EmailNotVerified`
- `InvalidToken`
- `TokenExpired`
- `TokenAlreadyUsed`
- `InvalidOtp`
- `OtpExpired`
- `OtpAttemptsExceeded`
- `PasswordPolicyViolation`
- `EmailAlreadyRegistered`
- `UserNotFound`
- `CurrentPasswordInvalid`
- `RefreshTokenInvalid`
- `RefreshTokenReused`

### 22.3 ProblemDetails mapping

- Validation errors: HTTP 400 `ValidationProblemDetails`.
- Authentication failures: HTTP 401 with `AuthErrorCode`.
- Authorization failures: HTTP 403.
- Missing resource: HTTP 404.
- Conflict: HTTP 409.
- Rate limit: HTTP 429 with retry-after.
- Server errors: HTTP 500 generic ProblemDetails.

## 23. Transaction and consistency rules

- Registration, token generation, and role assignment occur in one database transaction.
- Refresh rotation uses a transaction and row-level locking where possible.
- Email sending uses an outbox table or queued background job to avoid partial failures.
- Idempotency keys may be added for registration and password reset requests.
- All timestamps use UTC.

## 24. Authorization matrix

| Role | Browse | Purchase | Vendor Dashboard | Admin Dashboard | User Management | System Settings |
|---|---:|---:|---:|---:|---:|---:|
| Customer | Yes | Yes | No | No | No | No |
| Vendor | Yes | Yes | Yes | No | No | No |
| Support | Yes | Maybe | Read-only | Limited | Limited | No |
| Admin | Yes | Yes | Yes | Yes | Yes | Limited |
| SuperAdmin | Yes | Yes | Yes | Yes | Yes | Yes |

Policies should be claim-based where possible, with roles as the initial mechanism.

## 25. Account lifecycle and administration

Planned admin capabilities:
- List users with filters
- View user details and audit history
- Suspend/activate/deactivate user
- Unlock locked account
- Reset user password
- Force email verification
- Assign/remove roles
- Revoke all sessions
- Export user data
- Delete or anonymize account

These endpoints belong in a future `Admin` area and use the same auth services.

## 26. Additional auth-related flows

### 26.1 Email change
1. Require current password.
2. Send verification to new email.
3. On verification, update email and normalized email.
4. Rotate security stamp.
5. Revoke refresh tokens.

### 26.2 Phone verification
1. Send OTP to phone.
2. Verify OTP.
3. Mark phone confirmed.

### 26.3 Social login (future)
- Google, Apple, Facebook
- Account linking and unlinking
- Conflict handling for existing email

### 26.4 Passwordless login (future)
- Magic link or OTP-based login
- Device trust

### 26.5 API keys (future)
- For integrations and vendors
- Scoped permissions and rotation

## 27. Security hardening checklist

- Use Argon2id or ASP.NET Core password hasher with strong parameters.
- Enforce HTTPS and HSTS.
- Set security headers.
- Protect against brute force and credential stuffing.
- Use constant-time comparisons for tokens.
- Avoid logging secrets or tokens.
- Validate redirect URLs.
- Use anti-forgery on state-changing MVC requests.
- Store refresh tokens hashed.
- Rotate signing keys with a documented procedure.
- Run dependency vulnerability scans.
- Add SAST/DAST to CI.

## 28. Observability and audit

Metrics:
- Registration count
- Login success/failure count
- Lockout count
- Token refresh count
- OTP request/verification count
- Password reset count

Logs:
- Structured auth events with correlation IDs
- No PII beyond necessary identifiers
- Security events at warning/error level

Audit retention:
- Login audits retained for 90-180 days
- Admin actions retained longer

## 29. Performance and scalability

- Index all lookup fields.
- Cache role/permission lookups where appropriate.
- Keep access tokens stateless.
- Store refresh tokens in PostgreSQL initially; move to Redis if needed.
- Use async EF Core operations.
- Add pagination to admin user lists.

## 30. Accessibility and UX

- Form labels and autocomplete attributes.
- Keyboard navigation.
- Screen-reader-friendly validation messages.
- Clear password requirements.
- Non-color status indicators.
- Reasonable session timeout messaging.

## 31. Localization and content

- Use resource files for UI text.
- Localize email templates.
- Support Bengali and English as initial languages.
- Store user preferred language in profile.

## 32. DevOps and deployment

- Apply migrations via CI/CD or controlled startup.
- Use environment-specific configuration.
- Store secrets in secret manager.
- Add health checks for database and email provider.
- Document rollback procedure.
- Add smoke tests after deployment.

## 33. Data protection and compliance

- Obtain consent during registration.
- Provide privacy policy link.
- Support account deletion/anonymization.
- Define retention periods.
- Protect personal data in logs and backups.
- Document data processors and providers.

## 34. Migration and rollback plan

1. Backup database.
2. Apply identity migration.
3. Seed roles.
4. Deploy application.
5. Run smoke tests.
6. If failure, roll back application and database migration.
7. Verify no orphaned tokens.

## 35. Test data and fixtures

- Seed test users for each role in integration tests.
- Use isolated test database.
- Reset state between tests.
- Include expired token fixtures.
- Include locked account fixtures.

## 36. Definition of done

Implementation is not complete until:
- All planned endpoints are implemented.
- All views are bound to models.
- Migrations are reviewed and applied.
- Security checklist is satisfied.
- Unit and integration tests pass.
- Security tests pass.
- Documentation is updated.
- Code is formatted.
- No secrets are committed.

## 37. Acceptance criteria

The implementation is complete when:

1. A new customer or vendor can register and verify email.
2. A verified user can log in through MVC cookie auth.
3. A mobile/API client can log in and receive JWT + refresh token.
4. Refresh token rotation and reuse detection work.
5. Forgot/reset password works end to end.
6. Change password works and rotates security stamp.
7. Lockout and rate limiting protect auth endpoints.
8. Role-based authorization policies are enforced.
9. Login attempts are audited.
10. Unit and integration tests pass.
11. Documentation is updated to reflect implemented auth behavior.
