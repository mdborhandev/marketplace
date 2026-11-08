using Microsoft.AspNetCore.Mvc;

namespace market.Application.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => RedirectToRoute("Login");

    [HttpGet("/shop")]
    public IActionResult Shop() => View("Index");
}
