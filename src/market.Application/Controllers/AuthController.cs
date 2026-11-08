using Microsoft.AspNetCore.Mvc;

namespace market.Application.Controllers;

[Route("auth")]
public class AuthController : Controller
{
    [HttpGet("login", Name = "AuthLogin")]
    [HttpGet("/login", Name = "Login")]
    public IActionResult Login() => View();

    [HttpGet("register", Name = "AuthRegister")]
    [HttpGet("/register", Name = "Register")]
    public IActionResult Register() => View();
}
