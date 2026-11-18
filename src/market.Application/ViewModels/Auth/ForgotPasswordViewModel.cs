using System.ComponentModel.DataAnnotations;

namespace market.Application.ViewModels.Auth;

public sealed class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;
}