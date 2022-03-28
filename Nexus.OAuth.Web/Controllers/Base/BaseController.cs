using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Web;

namespace Nexus.OAuth.Web.Controllers.Base
{
    public class BaseController : Controller
    {
        protected internal const string DefaultRedirect = "/Home/Index";
        private static HttpClient? _apiClient;
        protected internal static Uri ApiUri => new(
#if DEBUG
              "https://localhost:44360/api/"
#else
""
#endif
            );
        protected internal static HttpClient ApiClient
        {
            get
            {

                if (_apiClient == null)
                {
                    _apiClient = new();

                    _apiClient.BaseAddress = ApiUri;
                    _apiClient.DefaultRequestHeaders
                        .Accept
                        .Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                return _apiClient;
            }
        }
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
