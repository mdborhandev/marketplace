namespace market.Core.Models.Identity;

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