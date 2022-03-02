using Newtonsoft.Json;
using Nexus.OAuth.Libary.Controllers.Base;
using Nexus.OAuth.Libary.Exceptions;
using Nexus.OAuth.Libary.Models;
using System.Net;
using System.Web;

namespace Nexus.OAuth.Libary.Controllers
{
    internal partial class OAuthController : BaseController
    {
        public async Task<AccessTokenResult> GetAccessToken(string client_id, string client_secret, string code, string? refresh_token, TokenType? type)
        {
            string url = $"{apiHost}OAuth/AcceessToken?" +
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

            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new InvalidOAuthException();
            }

            string responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AccessTokenResult>(responseString) ?? new();
        }
    }
}
