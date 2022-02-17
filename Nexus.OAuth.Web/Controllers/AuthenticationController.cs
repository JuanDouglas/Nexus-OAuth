using Microsoft.AspNetCore.Mvc;
using Nexus.OAuth.Web.Controllers.Base;
using System.Net;
using System.Web;

namespace Nexus.OAuth.Web.Controllers
{
    public class AuthenticationController : BaseController
    {
        public IActionResult Index(string? after)
        {
            if (XssValidation(after))
                return XssError();
           
            ViewBag.RedirectTo = after ?? "Home/Index";
            return View();
        }
        public IActionResult Authorize(string client_id)
        {
            if (XssValidation(client_id))
                return XssError();

            ViewBag.ClientId = client_id ?? string.Empty;
            return View();
        }

        public IActionResult Redirect()
        {
            return View();
        }
    }
}
