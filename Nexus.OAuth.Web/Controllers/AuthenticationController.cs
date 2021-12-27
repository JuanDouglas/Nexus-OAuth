using Microsoft.AspNetCore.Mvc;

namespace Nexus.OAuth.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Authorize(string client_id)
        {
            ViewData["ClientId"] = client_id ?? string.Empty;
            return View();
        }
    }
}
