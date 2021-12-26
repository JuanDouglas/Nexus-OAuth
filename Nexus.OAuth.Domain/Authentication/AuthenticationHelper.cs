using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Nexus.OAuth.Dal;
using Nexus.OAuth.Dal.Models;
using Nexus.OAuth.Dal.Models.Enums;
using Nexus.OAuth.Domain.Authentication.Exceptions;
using Nexus.Tools.Validations.Middlewares.Authentication;

namespace Nexus.OAuth.Domain.Authentication
{
    public class AuthenticationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        public const string AuthorizationHeader = "Authorization";

        /// <summary>
        /// 
        /// </summary>
        public const string ClientKeyHeader = "Client-Key";

        /// <summary>
        /// Valid if context authentication isValid.
        /// </summary>
        /// <param name="ctx">HttpContext for this request</param>
        /// <returns></returns>
        public static async Task<AuthenticationMidddleware.AuthenticationResult> ValidAuthenticationResultAsync(HttpContext ctx)
        {
            bool isValid = false;
            bool isConfirmed = false;

            TokenType tokenType;
            string token, secondToken, clientKey;

            try
            {
                (tokenType, token, secondToken, clientKey) = GetAuthorization(ctx);
            }
            catch (AuthenticationException)
            {
                return new(isValid, isConfirmed);
            }


            if (string.IsNullOrEmpty(token) ||
                string.IsNullOrEmpty(secondToken))
                return new(isValid, isConfirmed);


            using (OAuthContext db = new())
            {
                Dal.Models.Authentication? authentication = await (from fs in db.Authentications
                                                                   where fs.TokenType == tokenType &&
                                                                         fs.Token == token &&
                                                                         fs.IsValid
                                                                   select fs).FirstOrDefaultAsync();

                if (authentication?.ExpiresIn.HasValue ?? false)
                {
                    if (authentication.ExpiresIn > 0 &&
                        (DateTime.UtcNow - authentication.Date).TotalSeconds > authentication.ExpiresIn)
                    {
                        authentication.IsValid = false;
                        await db.SaveChangesAsync();
                    }
                }

                if (authentication?.FirstStepId.HasValue ?? false &&
                    authentication.IsValid)
                {
                    FirstStep firstStep = await (from fs in db.FirstSteps
                                                 where fs.Id == authentication.FirstStepId.Value
                                                 select fs).FirstOrDefaultAsync() ?? new();

                    isValid =
                        GeneralHelpers.ValidPassword(clientKey, firstStep?.ClientKey ?? string.Empty) &&
                        GeneralHelpers.ValidPassword(secondToken, firstStep?.Token ?? string.Empty);
                }

                if (authentication?.AuthorizationId.HasValue ?? false &&
                    !isValid &&
                    authentication.IsValid)
                {
                    Authorization firstStep = await (from fs in db.Authorizations
                                                     where fs.Id == authentication.AuthorizationId.Value
                                                     select fs).FirstOrDefaultAsync() ?? new();

                    //TODO: Implements Application Authentication Here
                }

                Account? account = await GetAccountAsync(tokenType, token);
                isConfirmed = account?.ConfirmationStatus > ConfirmationStatus.EmailSucess;
            }

            return new(isValid, isConfirmed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<Account?> GetAccountAsync(TokenType tokenType, string token)
        {
            Account? account = null;
            int accountId = 0;
            using (OAuthContext db = new())
            {

                Dal.Models.Authentication? authentication = await (from auth in db.Authentications
                                                                   where auth.IsValid &&
                                                                         auth.Token == token &&
                                                                         auth.TokenType == tokenType
                                                                   select auth).FirstOrDefaultAsync();

                #region Try Get AccountId
                if (authentication != null)
                {
                    #region Using Authorization
                    if (authentication.AuthorizationId.HasValue)
                    {
                        Authorization? authorization = await (from auth in db.Authorizations
                                                              where auth.Id == authentication.AuthorizationId.Value
                                                              select auth).FirstOrDefaultAsync();

                        accountId = authorization?.AccountId ?? 0;
                    }
                    #endregion

                    #region Using FirstStep
                    if (authentication.FirstStepId.HasValue)
                    {
                        FirstStep? firstStep = await (from fs in db.FirstSteps
                                                      where fs.Id == authentication.FirstStepId.Value
                                                      select fs).FirstOrDefaultAsync();

                        accountId = firstStep?.AccountId ?? 0;
                    }
                    #endregion
                }
                #endregion

                if (accountId != 0)
                {
                    account = await (from fs in db.Accounts
                                     where fs.Id == accountId
                                     select fs).FirstOrDefaultAsync();
                }

            }
            return account;
        }

        /// <summary>
        /// Get Authorization Tokens
        /// </summary>
        /// <param name="ctx">Http application context</param>
        /// <returns>Tokens of authentication. (TokenType, AuthenticationToken, ConfirmToken, ClientKey)</returns>
        /// <exception cref="AuthenticationException">IF invalid Authorization header informations</exception>
        public static (TokenType, string, string, string) GetAuthorization(HttpContext ctx)
        {
            string
                header = ctx.Request.Headers[AuthorizationHeader],
                clientKey = ctx.Request.Headers[ClientKeyHeader],
                firstToken = string.Empty,
                secondToken = string.Empty;

            if (string.IsNullOrEmpty(header))
                header = ctx.Request.Cookies[AuthorizationHeader] ?? string.Empty;

            if (string.IsNullOrEmpty(clientKey))
                clientKey = ctx.Request.Cookies[ClientKeyHeader] ?? string.Empty;

            string[] splited = header.Split(' ');

            if (splited.Length < 2)
                throw new AuthenticationException("Invalid Authentication Header!", header);

            string type = splited[0] ?? string.Empty;
            _ = Enum.TryParse(type, true, out TokenType tokenType);

            switch (tokenType)
            {
                case TokenType.Barear:
                    string[] tokens = splited[1].Split('.');

                    if (tokens.Length < 2)
                        throw new AuthenticationException("Invalid token type or invalid token structer!", header);

                    firstToken = tokens[0];
                    secondToken = tokens[1];
                    break;
                case TokenType.Basic:

                    break;
            }

            return (tokenType, firstToken, secondToken, clientKey);
        }

    }
}
