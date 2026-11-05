using Microsoft.AspNetCore.Mvc;

namespace market.Application.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

    [Route("design-system")]
    public IActionResult DesignSystem() => View();

}
