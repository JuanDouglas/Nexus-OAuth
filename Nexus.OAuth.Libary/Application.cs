using Nexus.OAuth.Libary.Base;
using Nexus.OAuth.Libary.Controllers;
using Nexus.OAuth.Libary.Controllers.Base;
using Nexus.OAuth.Libary.Models;
using Nexus.OAuth.Libary.Models.Enums;
using System.Web;

namespace Nexus.OAuth.Libary
{
    public sealed class Application : BaseClient
    {
        private const string webUrl = BaseController.webHost;
        private string clientId;
        private string clientSecret;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static OAuthController oauthController;
        private static AccountsController accountsController;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Application(string client_id, string secret)
        {
            clientId = client_id;
            clientSecret = secret;

            if (oauthController == null)
            {
                oauthController = new OAuthController();
            }

            if (accountsController == null)
            {
                accountsController = new AccountsController(ClientKey);
            }
        }

        public Uri GenerateAuthorizeUrl(Scope[] scopes, out string state)
        {
            state = GenerateToken(96);

            return GenerateAuthorizeUrl(scopes, state);
        }
        public Uri GenerateAuthorizeUrl(Scope[] scopes, string? state = null)
        {
            string scopesString = string.Empty;

            foreach (Scope scope in scopes)
                scopesString += Enum.GetName(typeof(Scope), scope);

            string url = $"{webUrl}Authentication/Authorize?" +
                $"&client_id={HttpUtility.UrlEncode(clientId)}" +
                $"&state={HttpUtility.UrlEncode(state ?? string.Empty)}" +
                $"&scopes={HttpUtility.UrlEncode(scopesString)}";

            return new Uri(url);
        }
        public async Task<AccessToken> GetAccessTokenAsync(string code, TokenType? tokenType)
        {
            AccessTokenResult result = await oauthController.GetAccessToken(clientId, clientSecret, code, null, tokenType);

            AccessToken accessToken = new(result);

            return accessToken;
        }

        public static async Task<Account> GetAccountAsync(AccessToken accessToken)
        {
            AccountResult result = await accountsController.GetAccountAsync($"{Enum.GetName(accessToken.TokenType)} {accessToken.Token}");

            Account account = new(result);

            return account;
        }
    }
}