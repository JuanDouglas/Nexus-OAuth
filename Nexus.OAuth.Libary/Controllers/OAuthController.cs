using Nexus.OAuth.Libary.Controllers.Base;
using Nexus.OAuth.Libary.Models;
using Nexus.OAuth.Libary.Models.Api;
using System.Net;
using System.Web;

namespace Nexus.OAuth.Libary.Controllers
{
    internal partial class OAuthController : Controller
    {
        protected internal override string BasePath => "OAuth/";

        public OAuthController(string clientKey) : base(clientKey)
        {
        }
        public OAuthController(string clientKey, string productName, string? productVersion) : base(clientKey, productName, productVersion)
        {
        }
        public async Task<AccessTokenResult> GetAccessToken(string client_id, string client_secret, string code, string? refresh_token, TokenType? type)
        {
            string url = $"{apiHost}{BasePath}AccessToken?" +
                $"code={HttpUtility.UrlEncode(code)}" +
                $"&client_id={HttpUtility.UrlEncode(client_id)}" +
                $"&client_secret={HttpUtility.UrlEncode(client_secret)}";

            if (!string.IsNullOrEmpty(refresh_token))
            {
                url += $"&refresh_token={HttpUtility.UrlEncode(refresh_token)}";
            }

            if (type != null)
            {
                url += $"&token_type={HttpUtility.UrlEncode(Enum.GetName(type ?? TokenType.Barear)?.ToLowerInvariant())}";
            }

            HttpRequestMessage request = defaultRequest;
            request.RequestUri = new Uri(url);

            var response = await httpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new InvalidOAuthException();
            }

            string responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AccessTokenResult>(responseString) ?? new();
        }

        public async Task<bool> AuthorizeAsync(ApiAuthorization authorization, string client_id, string scopes, string? state = null)
        {
            dynamic dd = await AuthorizeAndReturnAsync(authorization, client_id, scopes, false, state);
            return true;
        }

        public async Task<dynamic> AuthorizeAndReturnAsync(ApiAuthorization authorization, string client_id, string scopes, bool debug = true, string? state = null)
        {
            string url = $"{apiHost}{BasePath}Authorize?" +
                         $"client_id={HttpUtility.UrlEncode(client_id)}" +
                         $"scopes={HttpUtility.UrlEncode(scopes)}";

            AutoRedirect = debug;

            if (!string.IsNullOrEmpty(state))
            {
                url += $"&state={HttpUtility.UrlEncode(state)}";
            }

            httpClient.DefaultRequestHeaders.Authorization = new(authorization.ToString());
            HttpResponseMessage response = await httpClient.GetAsync(url);

            throw new NotImplementedException();
        }
    }
}
