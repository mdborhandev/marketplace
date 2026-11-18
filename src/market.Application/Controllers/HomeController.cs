using Microsoft.AspNetCore.Mvc;

namespace market.Application.Controllers;

public class HomeController : Controller
{
    [HttpGet("/")]
    public IActionResult Index() => RedirectToRoute("Login");

    [HttpGet("/shop")]
    public IActionResult Shop() => View("Index");

    [HttpGet("/components")]
    public IActionResult Components() => View();

    [HttpGet("/components/buttons")]
    public IActionResult ComponentButtons() => View("ComponentButtons");

    [HttpGet("/components/forms")]
    public IActionResult ComponentForms() => View("ComponentForms");

    [HttpGet("/components/cards")]
    public IActionResult ComponentCards() => View("ComponentCards");

    [HttpGet("/components/tables")]
    public IActionResult ComponentTables() => View("ComponentTables");

    [HttpGet("/components/overlays")]
    public IActionResult ComponentOverlays() => View("ComponentOverlays");
}