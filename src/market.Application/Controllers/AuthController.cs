using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using market.Application.ViewModels.Auth;
using market.Core.Common;
using market.Core.Interfaces.Services;

namespace market.Application.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpGet("login", Name = "AuthLogin")]
    [HttpGet("/login", Name = "Login")]
    public IActionResult Login(string? returnUrl = null)
        => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost("login", Name = "AuthLoginPost")]
    [HttpPost("/login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = new ClientContext(HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        var result = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe, client, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Invalid email or password.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, result.User!.Id.ToString()),
            new(ClaimTypes.Email, result.User.Email),
            new(ClaimTypes.Name, result.User.FullName ?? result.User.Email)
        };
        foreach (var role in result.Roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var identity = new ClaimsIdentity(claims, "market.Cookie");
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(30) : DateTimeOffset.UtcNow.AddHours(12)
        };

        await HttpContext.SignInAsync("market.Cookie", principal, properties);

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("register", Name = "AuthRegister")]
    [HttpGet("/register", Name = "Register")]
    public IActionResult Register(string? returnUrl = null)
        => View(new RegisterViewModel { ReturnUrl = returnUrl });

    [HttpPost("register", Name = "AuthRegisterPost")]
    [HttpPost("/register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (!model.AcceptTerms)
        {
            ModelState.AddModelError(nameof(model.AcceptTerms), "You must accept the terms.");
            return View(model);
        }

        var client = new ClientContext(HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        var result = await _authService.RegisterAsync(model.FullName, model.StoreName, model.Email, model.Password, model.Role, client, cancellationToken);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, result.Message ?? "Registration failed.");
            return View(model);
        }

        return RedirectToAction(nameof(VerificationSent), new { email = model.Email });
    }

    [HttpGet("/verification-sent")]
    public IActionResult VerificationSent(string email)
        => View(model: email);

    [HttpPost("/logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (Guid.TryParse(userIdValue, out var userId))
            await _authService.RevokeAsync(userId, null, cancellationToken);

        await HttpContext.SignOutAsync("market.Cookie");
        return RedirectToRoute("Login");
    }

    [HttpGet("/forgot-password")]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost("/forgot-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = new ClientContext(HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        await _authService.ForgotPasswordAsync(model.Email, client, cancellationToken);
        return View("ForgotPasswordConfirmation");
    }

    [HttpGet("/reset-password")]
    public IActionResult ResetPassword(string email, string token)
        => View(new ResetPasswordViewModel { Email = email, Token = token });

    [HttpPost("/reset-password")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return View(model);

        var client = new ClientContext(HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent.ToString());
        var success = await _authService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword, client, cancellationToken);
        if (!success)
        {
            ModelState.AddModelError(string.Empty, "Invalid or expired reset token.");
            return View(model);
        }

        return RedirectToAction(nameof(Login));
    }

    [HttpGet("/verify-email")]
    public async Task<IActionResult> VerifyEmail(string email, string token, CancellationToken cancellationToken)
    {
        var success = await _authService.VerifyEmailAsync(email, token, cancellationToken);
        return View(model: success);
    }
}