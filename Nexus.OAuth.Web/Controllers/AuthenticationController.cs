using Microsoft.AspNetCore.Mvc;

namespace Nexus.OAuth.Web.Controllers
{

    public class AuthenticationController : Controller
    {
#warning Valid Anti XSS Attack
        public IActionResult Index(string? redirect)
        {
            ViewBag.RedirectTo = redirect ?? "Home/Index";
            return View();
        }

#warning Valid Anti XSS Attack
        public IActionResult Authorize(string client_id)
        {
            ViewData["ClientId"] = client_id ?? string.Empty;
            return View();
        }

        public IActionResult Redirect()
        {
            return View();
        }
    }
}
