using Microsoft.AspNetCore.Mvc;
using Nexus.OAuth.Web.Controllers.Base;
using Nexus.OAuth.Web.Models;
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
        public IActionResult Authorize(string client_id, string? state, string? scopes)
        {
            if (XssValidation(client_id) ||
                XssValidation(scopes) ||
                XssValidation(state))
                return XssError();

            if (!string.IsNullOrEmpty(scopes))
            {

            }

            ViewBag.ClientId = client_id ?? string.Empty;
            ViewBag.Scopes = scopes ?? Enum.GetName(Scope.User);
            ViewBag.State = state ?? string.Empty;
            return View();
        }

        public IActionResult Redirect()
        {
            return View();
        }
    }
}