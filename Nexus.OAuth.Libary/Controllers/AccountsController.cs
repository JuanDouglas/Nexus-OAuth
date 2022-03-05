using Nexus.OAuth.Libary.Controllers.Base;
using System.Net;
using System.Net.Http.Headers;

namespace Nexus.OAuth.Libary.Controllers
{
    internal class AccountsController : Controller
    {
        public AccountsController(string clientKey) : base(clientKey)
        {

        }
        public async Task<AccountResult> GetAccountAsync(string authorization)
        {
            string url = $"{apiHost}Accounts/MyAccount";
            HttpRequestMessage request = defaultRequest;
            request.Headers.Authorization = new AuthenticationHeaderValue(authorization);

            var response = await httpClient.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedException();
            }

            string responseString = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<AccountResult>(responseString) ?? new();
        }
    }
}
