using Microsoft.AspNetCore.Mvc;

namespace Nexus.OAuth.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
