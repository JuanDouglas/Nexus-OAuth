using Microsoft.AspNetCore.Mvc;

namespace Nexus.OAuth.Web.Controllers;
public class MetricsController : Controller
{
    public IActionResult Requests()
    {
        return View();
    }
}