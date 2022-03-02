using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Nexus.OAuth.Libary.Controllers.Base
{
    internal class BaseController
    {
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

        protected internal HttpClient httpClient => new() { };
        protected internal HttpRequestMessage defaultRequest => new();
    }
}
