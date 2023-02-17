using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Web;

namespace Nexus.OAuth.Web.Controllers.Base;
public class BaseController : Controller
{
    protected internal const string DefaultRedirect = "/Applications";

    public BaseController() : base()
    {
        ViewBag.Culture = "pt-BR";
    }

    public bool XssValidation(ref string? str)
    {
        str = HttpUtility.UrlDecode(str ?? string.Empty);
        str = str.Replace($"https://{Request.Host.Value}", string.Empty);
        str = str.Replace($"http://{Request.Host.Value}", string.Empty);
        string last = str;
        str = HttpUtility.HtmlEncode(str);

        if (str != last)
        {
            return true;
        }

        return false;
    }

    public IActionResult XssError()
        => StatusCode((int)HttpStatusCode.NotAcceptable, "What are you trying to do?");
}