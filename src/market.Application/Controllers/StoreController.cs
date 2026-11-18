using Microsoft.AspNetCore.Mvc;

namespace market.Application.Controllers;

public class StoreController : Controller
{
    [HttpGet("/", Name = "StoreHome")]
    public IActionResult Index() => View();

    [HttpGet("/store")]
    public IActionResult Store() => View("Index");
}