using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

namespace Nexus.OAuth.Web.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly char[] notAcceptebles = new char[] { '<', '>', '"', '\'', };
#warning Valid Anti XSS Attack
        public IActionResult Index(string? redirect)
        {
            if (XssValidation(redirect))
            {
                return XssError();
            }

            ViewBag.RedirectTo = redirect ?? "Home/Index";
            return View();
        }
        public IActionResult Authorize(string client_id)
        {
            if (XssValidation(client_id))
            {
                return XssError();
            }

            ViewBag.ClientId = client_id ?? string.Empty;
            return View();
        }

        public IActionResult Redirect()
        {
            return View();
        }

#warning Valid Anti XSS Attack
        public bool XssValidation(string? str) {
            str = HttpUtility.UrlDecode(str);

            if (string.IsNullOrEmpty(str))
            {
                return false;
            }

            foreach (var item in notAcceptebles)
            {
                if (str.Contains(item))
                    return true;
            }

            return false;
        }

        public IActionResult XssError() {
            return StatusCode((int)HttpStatusCode.NotAcceptable, "What are you trying to do?");
        }
    }
}
