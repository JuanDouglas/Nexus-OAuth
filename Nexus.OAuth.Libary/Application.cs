using Nexus.OAuth.Libary.Base;
using Nexus.OAuth.Libary.Controllers;
using Nexus.OAuth.Libary.Controllers.Base;
using Nexus.OAuth.Libary.Models;
using Nexus.OAuth.Libary.Models.Enums;
using System.Web;

namespace Nexus.OAuth.Libary
{
    /// <summary>
    /// OAuth Application, for more see https://docs.nexus-company.tech/OAuth
    /// </summary>
    public sealed class Application : BaseClient
    {
        private const string webUrl = BaseController.webHost;
        private string clientId;
        private string clientSecret;
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static OAuthController oauthController;
        private static AccountsController accountsController;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Create a new appication using your Nexus OAuth application
        /// </summary>
        /// <param name="clientId">Your application client id (get more information in https://oauth.nexus-company.tech/Applications)</param>
        /// <param name="secret"></param>
        public Application(string clientId, string secret)
        {
            this.clientId = clientId;
            clientSecret = secret;

            oauthController = new OAuthController(ClientKey);
            accountsController = new AccountsController(ClientKey);
        }

        /// <summary>
        /// Create a new appication using your Nexus OAuth application
        /// </summary>
        /// <param name="clientId">Your application client id (get more information in https://oauth.nexus-company.tech/Applications)</param>
        /// <param name="secret"></param>
        /// <param name="productName"></param>
        /// <param name="productVersion"></param>
        public Application(string clientId, string secret, string productName, string? productVersion) : this(clientId, secret)
        {
            oauthController.ProductName = productName;
            oauthController.ProductVersion = productVersion;
        }

        /// <summary>
        /// Generate authorize URL with a genereted state code.
        /// </summary>
        /// <param name="scopes">Necessary scopes</param>
        /// <param name="state">new state token</param>
        /// <returns>Uri for authorization</returns>
        public Uri GenerateAuthorizeUrl(Scope[] scopes, out string state)
        {
            state = GenerateToken(96);

            return GenerateAuthorizeUrl(scopes, state);
        }

        /// <summary>
        /// Generete a new URL for authorization
        /// </summary>
        /// <param name="scopes">Necessary scopes</param>
        /// <param name="state">Your state token</param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="code"></param>
        /// <param name="tokenType"></param>
        /// <returns></returns>
        public async Task<AccessToken> GetAccessTokenAsync(string code, TokenType? tokenType)
        {
            AccessTokenResult result = await oauthController.GetAccessToken(clientId, clientSecret, code, null, tokenType);

            AccessToken accessToken = new(result);

            return accessToken;
        }

        public async Task<Account> GetAccountAsync(AccessToken access)
        {
            AccountResult result = await accountsController.GetAccountAsync(access.TokenType, access.Token);

            Account account = new(result);

            return account;
        }
    }
}