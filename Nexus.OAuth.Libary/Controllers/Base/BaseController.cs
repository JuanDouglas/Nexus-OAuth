global using Newtonsoft.Json;
global using Nexus.OAuth.Libary.Models.Api.Result;
global using Nexus.OAuth.Libary.Exceptions;

namespace Nexus.OAuth.Libary.Controllers.Base
{
    internal abstract class BaseController
    {
        private const string Version = "1.0";
        protected internal const string apiHost =
#if DEBUG
        "https://localhost:44360/api/";
#else
        "https://auth.nexus-company.tech/api/";
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
        protected internal virtual HttpClient httpClient
        {
            get
            {
                HttpClient client = new(new HttpClientHandler()
                {
                    AllowAutoRedirect = AutoRedirect
                });
                client.BaseAddress = new Uri(apiHost);
                client.DefaultRequestHeaders.UserAgent.Clear();
                client.DefaultRequestHeaders.Add("User-Agent", UserAgent);

                return client;
            }
        }
        protected internal virtual HttpRequestMessage defaultRequest => new();
    }
}
