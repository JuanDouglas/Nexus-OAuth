using Nexus.OAuth.Libary.Controllers.Base;
using Nexus.OAuth.Libary.Models;
using System.Net;
using System.Web;

namespace Nexus.OAuth.Libary.Controllers
{
    internal class AuthenticationsController : Controller
    {
        protected internal override string BasePath => "Authentications/";
        public AuthenticationsController(string clientKey) : base(clientKey)
        {

        }
        public async Task<FirsStepResult> FirsStepAsync(string user, string? redirect = null)
        {
            string url = $"{apiHost}{BasePath}FirstStep?user={HttpUtility.UrlEncode(user)}" +
                $"&redirect={HttpUtility.UrlEncode(redirect)}";

            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new AuthenticationException();
            }

            string responseString = await response.Content.ReadAsStringAsync();

            FirsStepResult result = JsonConvert.DeserializeObject<FirsStepResult>(responseString);
            return result;
        }

        public async Task<AuthenticationResult> SecondStepAsync(string pwd, string token, int fs_id, TokenType? type = TokenType.Barear)
        {
            string url = $"{apiHost}{BasePath}SecondStep?pwd={HttpUtility.UrlEncode(pwd)}" +
                $"&token={HttpUtility.UrlEncode(token)}" +
                $"&fs_id={fs_id}";

            if (type != null)
            {
                url += $"&type={HttpUtility.UrlEncode(Enum.GetName(type ?? TokenType.Barear))}";
            }

            HttpResponseMessage response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new AuthenticationException();
            }

            string responseString = await response.Content.ReadAsStringAsync();

            AuthenticationResult result = JsonConvert.DeserializeObject<AuthenticationResult>(responseString);

            return result;
        }
    }
}
