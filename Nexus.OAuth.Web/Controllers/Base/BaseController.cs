using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Web;

namespace Nexus.OAuth.Web.Controllers.Base
{
    public abstract class BaseController : Controller
    {

        private readonly char[] notAcceptebles = new char[] { '<', '>', '"', '\'', };
#warning Valid Anti XSS Attack
        public bool XssValidation(string? str)
        {
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

        public IActionResult XssError()
        {
            return StatusCode((int)HttpStatusCode.NotAcceptable, "What are you trying to do?");
        }
    }
}
