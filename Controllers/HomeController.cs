using Microsoft.AspNetCore.Mvc;

namespace RideTracker.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Customer"); // Default route
    }

    [Route("driver")]
    public IActionResult Driver()
    {
        return View();
    }

    [Route("customer")]
    public IActionResult Customer()
    {
        return View();
    }
}