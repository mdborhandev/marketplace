using Microsoft.AspNetCore.Mvc;

namespace market.Application.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    [HttpGet("/shop")]
    public IActionResult Shop() => View("Index");

    [HttpGet("/components")]
    public IActionResult Components() => View();
}