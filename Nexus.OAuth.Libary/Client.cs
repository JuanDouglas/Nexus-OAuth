using Nexus.OAuth.Libary.Base;
using Nexus.OAuth.Libary.Controllers;
using Nexus.OAuth.Libary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Libary
{
    public class Client : BaseClient
    {
        private string Authorization { get; set; }
        internal Client(string authorization) : base()
        {
            Authorization = authorization;
        }

        public Client(string clientKey, TokenType type, string firstStepToken, string authorizationToken)
        {
            ClientKey = clientKey;
            Authorization = $"{Enum.GetName(type)} {authorizationToken}.{firstStepToken}";
        }

        public static async Task<Client> LoginAsync(string user, string pwd, TokenType tokenType = TokenType.Barear)
        {
            AuthenticationsController authenticationController = new(ClientKey);
            FirsStepResult firsStep = await authenticationController.FirsStepAsync(user);
            AuthenticationResult authentication = await authenticationController.SecondStepAsync(pwd, firsStep.Token, firsStep.Id, tokenType);

            Client client = new(ClientKey, authentication.TokenType, firsStep.Token, authentication.Token);
            return client;
        }

        public async Task<bool> AuthorizeAsync(string clientId, string scopes)
        {
            OAuthController authController = new();
            Models.Api.ApiAuthorization auth = new(Authorization);
            await authController.AuthorizeAsync(auth, clientId, scopes);
            return false;
        }
    }
}
