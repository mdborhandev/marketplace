namespace market.Core.Enums;

public enum AuthErrorCode
{
    InvalidCredentials = 0,
    AccountLocked = 1,
    AccountSuspended = 2,
    EmailNotVerified = 3,
    InvalidToken = 4,
    TokenExpired = 5,
    TokenAlreadyUsed = 6,
    InvalidOtp = 7,
    OtpExpired = 8,
    OtpAttemptsExceeded = 9,
    PasswordPolicyViolation = 10,
    EmailAlreadyRegistered = 11,
    UserNotFound = 12,
    CurrentPasswordInvalid = 13,
    RefreshTokenInvalid = 14,
    RefreshTokenReused = 15
}