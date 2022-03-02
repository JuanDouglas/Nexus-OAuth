using Nexus.OAuth.Libary.Base;
using Nexus.OAuth.Libary.Controllers;
using Nexus.OAuth.Libary.Models;
using static Nexus.OAuth.Libary.Controllers.OAuthController;

namespace Nexus.OAuth.Libary
{
    public class Application : BaseClient
    {
        private string clientId;
        private string clientSecret;
        private OAuthController oauthController;
        public Application(string client_id, string secret)
        {
            clientId = client_id;
            clientSecret = secret;
            oauthController = new OAuthController();
        }
        public void RequestAuthorization()
        {

        }

        public async Task<AccessToken> GetAccessToken(string code, TokenType? tokenType)
        {
            AccessTokenResult result = await oauthController.GetAccessToken(clientId, clientSecret, code, null, tokenType);

            AccessToken accessToken = new(result);

            return accessToken;
        }

        public Client TransformUserInClient(AccessToken accessToken)
        {

        }
    }
}