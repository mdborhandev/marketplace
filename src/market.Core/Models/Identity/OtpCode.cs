using market.Core.Enums;

namespace market.Core.Models.Identity;

public sealed class OtpCode
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public string Target { get; set; } = string.Empty;
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