using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

namespace Nexus.OAuth.Web.Controllers.Base
{
    public class BaseController : Controller
    {
        protected internal const string DefaultRedirect = "/Applications";

        private readonly char[] notAcceptebles = new char[] { '<', '>', '"', '\'', };
#warning Valid Anti XSS Attack

        public BaseController() : base()
        {
        }

        public bool XssValidation(ref string? str)
        {
            str = HttpUtility.UrlDecode(str ?? string.Empty);
            str = str.Replace($"https://{Request.Host.Value}", string.Empty);
            str = str.Replace($"http://{Request.Host.Value}", string.Empty);

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

        public IActionResult XssError()
        {
            return StatusCode((int)HttpStatusCode.NotAcceptable, "What are you trying to do?");
        }
    }
}
