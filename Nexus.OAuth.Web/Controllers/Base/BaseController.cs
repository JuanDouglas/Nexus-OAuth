using Microsoft.AspNetCore.Mvc;

namespace Nexus.OAuth.Web.Controllers.Base
{
    public class BaseController : Controller
    {
        private static HttpClient? _apiClient;
        protected internal static  Uri ApiUri => new (
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
                }

                return _apiClient;
            }
        }
    }
}
