using Nexus.OAuth.Libary.Base;
using Nexus.OAuth.Libary.Controllers;
using Nexus.OAuth.Libary.Models;
using Nexus.OAuth.Libary.Models.Api.Result;
using static Nexus.OAuth.Libary.Controllers.OAuthController;

namespace Nexus.OAuth.Libary
{
    public class Application : BaseClient
    {
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
        public async Task<AccessToken> GetAccessToken(string code, TokenType? tokenType)
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