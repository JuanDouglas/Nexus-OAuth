using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Nexus.OAuth.Dal;
using Nexus.OAuth.Dal.Models;
using Nexus.OAuth.Dal.Models.Enums;
using Nexus.OAuth.Domain.Authentication.Exceptions;
using static Nexus.Tools.Validations.Middlewares.Authentication.AuthenticationMidddleware;

namespace Nexus.OAuth.Domain.Authentication
{
    public class AuthenticationHelper
    {
        private readonly string ConnectionString;
        public AuthenticationHelper(string conn)
        {
            ConnectionString = conn;
        }

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
        public async Task<AuthenticationResult> ValidAuthenticationResultAsync(HttpContext ctx)
        {
            bool isValid = false;
            bool isConfirmed = false;
            bool isOwner = false;

            int level = -1;
            TokenType tokenType;
            string[] tokens;
            string clientKey;

            using OAuthContext db = new(ConnectionString);

            try
            {
                (tokenType, tokens, clientKey) = GetAuthorization(ctx);
            }
            catch (AuthenticationException)
            {
                return new(isValid, isConfirmed);
            }

            if (tokens.Length < 1)
                return new(isValid, isConfirmed);

            if (string.IsNullOrEmpty(tokens[0]))
                return new(isValid, isConfirmed);

            Dal.Models.Authentication? authentication = await (from fs in db.Authentications
                                                               where fs.TokenType == tokenType &&
                                                                     fs.Token == tokens[0] &&
                                                                     fs.IsValid
                                                               select fs).FirstOrDefaultAsync();

            if (authentication == null)
                return new(isValid, isConfirmed);

            if (authentication.ExpiresIn.HasValue)
            {
                if (authentication.ExpiresIn > 0 &&
                    (DateTime.UtcNow - authentication.Date).TotalSeconds > authentication.ExpiresIn)
                {
                    authentication.IsValid = false;
                    await db.SaveChangesAsync();
                }
            }

            if (authentication.FirstStepId.HasValue &&
                authentication.IsValid &&
                tokens.Length > 1)
            {
                FirstStep firstStep = await (from fs in db.FirstSteps
                                             where fs.Id == authentication.FirstStepId.Value
                                             select fs).FirstOrDefaultAsync() ?? new();

                isValid =
                    GeneralHelpers.ValidPassword(clientKey, firstStep?.ClientKey ?? string.Empty) &&
                    GeneralHelpers.ValidPassword(tokens[1] ?? string.Empty, firstStep?.Token ?? string.Empty);

                level = int.MaxValue;
                isOwner = true;
            }

            if (authentication.AuthorizationId.HasValue &&
                !isValid &&
                authentication.IsValid &&
                tokens.Length == 1)
            {
                Authorization? firstStep = await (from fs in db.Authorizations
                                                  where fs.Id == authentication.AuthorizationId
                                                  select fs).FirstOrDefaultAsync() ?? new();

                isValid =
                    firstStep.IsValid &&
                    GeneralHelpers.ValidPassword(clientKey, firstStep?.ClientKey ?? string.Empty);

                level = (int)firstStep.Scopes.OrderByDescending(ord => ord)
                                             .FirstOrDefault();
            }

            if (authentication.QrCodeAuthorizationId.HasValue &&
                !isValid &&
                authentication.IsValid &&
                tokens.Length > 1)
            {
                QrCodeAuthorization firstStep = await (from fs in db.QrCodeAuthorizations
                                                       where fs.Id == authentication.QrCodeAuthorizationId
                                                       select fs).FirstOrDefaultAsync() ?? new();

                QrCodeReference? reference = await (from rf in db.QrCodes
                                                    where rf.Id == firstStep.QrCodeReferenceId
                                                    select rf).FirstOrDefaultAsync();

                isValid =
                    GeneralHelpers.ValidPassword(clientKey, reference?.ClientKey ?? string.Empty) &&
                    GeneralHelpers.ValidPassword(tokens[1] ?? string.Empty, reference?.ValidationToken ?? string.Empty);

                level = int.MaxValue;
                isOwner = true;
            }

            Account? account = await GetAccountAsync(tokenType, tokens[0], db);
            isConfirmed = account?.ConfirmationStatus > ConfirmationStatus.EmailSucess;


            return new(isValid, isConfirmed, level)
            {
                IsOwner = isOwner
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Account?> GetAccountAsync(TokenType tokenType, string token, OAuthContext db)
        {
            Account? account = null;
            int accountId = 0;

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

                #region Using QrCode
                if (authentication.QrCodeAuthorizationId.HasValue)
                {
                    QrCodeAuthorization? firstStep = await (from fs in db.QrCodeAuthorizations
                                                            where fs.Id == authentication.QrCodeAuthorizationId.Value
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
            return account;
        }

        /// <summary>
        /// Get Authorization Tokens
        /// </summary>
        /// <param name="ctx">Http application context</param>
        /// <returns>Tokens of authentication. (<see cref="TokenType"/> tokenType, <see cref="string"/> authenticationToken, <see cref="string"/> confirmToken, <see cref="string"/> clientKey)</returns>
        /// <exception cref="AuthenticationException">IF invalid Authorization header informations</exception>
        public static (TokenType, string[], string) GetAuthorization(HttpContext ctx)
        {
            string[] tokens = Array.Empty<string>();

            string
                header = ctx.Request.Headers[AuthorizationHeader],
                clientKey = ctx.Request.Headers[ClientKeyHeader];

            AuthenticationException exception = new($"Request for ip {ctx.Connection.RemoteIpAddress} failed with authentication Header or cookie!", header);

            if (string.IsNullOrEmpty(header))
                header = ctx.Request.Cookies[AuthorizationHeader] ?? string.Empty;

            if (string.IsNullOrEmpty(clientKey))
                clientKey = ctx.Request.Cookies[ClientKeyHeader] ?? string.Empty;

            string[] splited = header.Split(' ');

            if (splited.Length < 2)
                throw exception;

            string type = splited[0] ?? string.Empty;
            _ = Enum.TryParse(type, true, out TokenType tokenType);

            switch (tokenType)
            {
                case TokenType.Barear:
                    tokens = splited[1].Split('.');

                    if (tokens.Length < 1)
                        throw exception;

                    if (string.IsNullOrEmpty(tokens[0]))
                        throw exception;

                    break;
                case TokenType.Basic:

                    break;
            }

            return (tokenType, tokens, clientKey);
        }

    }
}
