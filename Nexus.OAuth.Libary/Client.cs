using Nexus.OAuth.Libary.Base;
using Nexus.OAuth.Libary.Controllers;
using Nexus.OAuth.Libary.Models;

namespace Nexus.OAuth.Libary
{
    public class Client : BaseClient
    {
        private string Authorization { get; set; }
        internal Client(string clientKey, string authorization) : base()
        {
            ClientKey = clientKey;
            Authorization = authorization;
        }

        public Client(string clientKey, TokenType type, string firstStepToken, string authorizationToken)
        {
            ClientKey = clientKey;
            Authorization = $"{Enum.GetName(type)} {authorizationToken}.{firstStepToken}";
        }

        public static async Task<Client> ClientByLoginAsync(string user, string pwd, string clientKey, TokenType tokenType = TokenType.Barear)
        {
            (tokenType, string fsToken, string authToken) = await LoginAsync(user, pwd, clientKey, tokenType);
            Client client = new(clientKey, tokenType, fsToken, authToken);
            return client;
        }

        private static async Task<(TokenType, string, string)> LoginAsync(string user, string pwd, string clientKey, TokenType tokenType)
        {
            AuthenticationsController authenticationController = new(clientKey);
            FirsStepResult firsStep = await authenticationController.FirsStepAsync(user);
            AuthenticationResult authentication = await authenticationController.SecondStepAsync(pwd, firsStep.Token, firsStep.Id, tokenType);

            return (authentication.TokenType, firsStep.Token, authentication.Token);
        }
        public async Task<bool> AuthorizeAsync(string clientId, string scopes)
        {
            OAuthController authController = new(ClientKey);
            Models.Api.ApiAuthorization auth = new(Authorization);
            await authController.AuthorizeAsync(auth, clientId, scopes);
            return false;
        }
    }
}
