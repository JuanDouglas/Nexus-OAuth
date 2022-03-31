using Microsoft.AspNetCore.Mvc;

namespace Nexus.OAuth.Web.Controllers
{
    public class ApplicationsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
