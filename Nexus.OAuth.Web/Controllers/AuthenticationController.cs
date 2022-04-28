using Microsoft.AspNetCore.Mvc;
using Nexus.OAuth.Web.Controllers.Base;
using Nexus.OAuth.Web.Models.Enums;

namespace Nexus.OAuth.Web.Controllers
{
    public class AuthenticationController : BaseController
    {
        public IActionResult Index(string? after)
        {
            after ??= DefaultRedirect;
            if (XssValidation(ref after))
                return XssError();

            ViewBag.RedirectTo = after;
            return View();
        }
        public IActionResult Authorize(string client_id, string? state, string? scopes)
        {
            if (XssValidation(ref client_id) ||
                XssValidation(ref scopes) ||
                XssValidation(ref state))
                return XssError();

            Scope[] scopeArray = new[] { Scope.User };

            if (!string.IsNullOrEmpty(scopes))
            {
                bool isValid = GetScopes(out scopeArray, scopes);

                if (!isValid)
                {
                    return BadRequest("Verify scopes");
                }
            }

            ViewBag.ClientId = client_id ?? string.Empty;
            ViewBag.Scopes = scopes ?? "user";
            ViewBag.State = state ?? string.Empty;
            return View();
        }

        public IActionResult Redirect()
        {
            return View();
        }

        [NonAction]
        private static bool GetScopes(out Scope[] scopes, string str)
        {
            string[] scopesStrings = str.Split(',');
            scopes = Array.Empty<Scope>();

            List<Scope> scopesList = new();
            foreach (string strScope in scopesStrings)
            {
                bool isValid = Enum.TryParse(strScope, true, out Scope scope);

                if (!isValid ||
                    scopesList.Contains(scope))
                    return false;

                scopesList.Add(scope);
            }

            scopes = scopesList.ToArray();
            return true;
        }
    }
}