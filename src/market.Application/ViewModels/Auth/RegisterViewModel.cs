using System.ComponentModel.DataAnnotations;

namespace market.Application.ViewModels.Auth;

public sealed class RegisterViewModel
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 2)]
    public string? StoreName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 8)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(Password))]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = "Customer";

    public bool AcceptTerms { get; set; }

    public string? ReturnUrl { get; set; }
}