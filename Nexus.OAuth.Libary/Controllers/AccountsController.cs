using Nexus.OAuth.Libary.Controllers.Base;
using Nexus.OAuth.Libary.Models;
using System.Net;

namespace Nexus.OAuth.Libary.Controllers;
internal class AccountsController : Controller
{
    public AccountsController(string clientKey, string authorization, TokenType tokenType)
        : base(clientKey, authorization, tokenType)
    {

    }
    public async Task<AccountResult> GetAccountAsync(TokenType tokenType, string authorization)
    {
        string url = $"{apiHost}Account/MyAccount";
        HttpRequestMessage request = defaultRequest;

        request.RequestUri = new(url);
        request.Headers.Authorization = new AuthenticationHeaderValue(Enum.GetName(tokenType) ?? "Barear", authorization);

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