using Nexus.OAuth.Libary.Controllers.Base;
using Nexus.OAuth.Libary.Models;
using System.Net;

namespace Nexus.OAuth.Libary.Controllers;
internal class AccountsController : AuthorizedController
{
    public AccountsController(string clientKey, string authorization, TokenType tokenType)
        : base(clientKey, authorization, tokenType)
    {

    }
    public async Task<AccountResult> MyAccountAsync()
    {
        string url = $"{apiHost}Account/MyAccount";
        HttpRequestMessage request = defaultRequest;

        request.RequestUri = new(url);
        var response = await httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException();
        }

        string responseString = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<AccountResult>(responseString) ??
            throw new InternalErrorException();
    }
}