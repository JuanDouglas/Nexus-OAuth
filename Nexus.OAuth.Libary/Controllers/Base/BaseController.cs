global using Newtonsoft.Json;
global using Nexus.OAuth.Libary.Models.Api.Result;
global using Nexus.OAuth.Libary.Exceptions;
using System.Net.Http.Headers;

namespace Nexus.OAuth.Libary.Controllers.Base
{
    internal abstract class BaseController
    {
        private const string Version = "1.0";
        protected internal const string apiHost =
#if DEBUG
        "https://localhost:44360/api/";
#else
        "https://nexus-oauth-api.azurewebsites.net/api/";
#endif

        protected internal const string webHost =
#if DEBUG
        "https://localhost:44337/";
#else
        "https://oauth.nexus-company.tech/";
#endif

        public bool AutoRedirect { get; set; } = true;
        protected internal virtual string BasePath => "";
        public string UserAgent { get; set; } = $"Nexus Libary Client {Version}";
        protected internal virtual HttpClient httpClient => new();
        protected internal virtual HttpRequestMessage defaultRequest
        {
            get
            {
                HttpRequestMessage request = new(HttpMethod.Get, webHost);
                request.Headers.UserAgent.Add(new ProductInfoHeaderValue(UserAgent));

                return request;
            }
        }
    }
}
