using Microsoft.AspNetCore.Mvc;

namespace PoshakGhor.Application.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => View();

}
