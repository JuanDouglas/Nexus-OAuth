using Microsoft.AspNetCore.Cors;
using Nexus.OAuth.Server.Exceptions;
using Authorization = Nexus.OAuth.Dal.Models.Authorization;

namespace Nexus.OAuth.Server.Controllers.Base;

/// <summary>
/// Base Application Controller
/// </summary>
[RequireHttps]
[ApiController]
[Route("api/[controller]")]
[RequireAuthentication(RequireAccountValidation = true, ShowView = true)]
public class ApiController : ControllerBase
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
    /// 
    /// </summary>
    public const string UserAgentHeader = "User-Agent";

    protected static internal readonly OAuthContext db = new();

    /// <summary>
    /// Request Client Account 
    /// </summary>
    protected internal Account? ClientAccount
    {
        get
        {
            (TokenType tokenType, string token, _, _) = GetAuthorization(HttpContext);

            Task<Account?> getAccount = GetAccountAsync(tokenType, token);
            getAccount.Wait();




            return getAccount.Result;
        }
    }
    /// <summary>
    /// Client User-Agent
    /// </summary>
    public string UserAgent { get => Request.Headers.UserAgent.ToString(); }

    /// <summary>
    /// Client Remote Ip Adress
    /// </summary>
    public IPAddress? RemoteIpAdress { get => HttpContext.Connection.RemoteIpAddress; }

    /// <summary>
    /// Transform string password in string hash 
    /// </summary>
    /// <param name="password">String password</param>
    /// <returns>New hash by password</returns>
    [NonAction]
    public static string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Valid password by password hash.
    /// </summary>
    /// <param name="password">Password</param>
    /// <param name="hash">Password hash.</param>
    /// <returns>Password is valid</returns>
    [NonAction]
    public static bool ValidPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    /// <summary>
    /// Get Authorization Tokens
    /// </summary>
    /// <param name="ctx">Http application context</param>
    /// <returns>Tokens of authentication.</returns>
    /// <exception cref="AuthenticationException">IF invalid Authorization header informations</exception>
    protected internal static (TokenType, string, string, string) GetAuthorization(HttpContext ctx)
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

    protected internal static async Task<Account?> GetAccountAsync(TokenType tokenType, string token)
    {
        Account account = null;
        int accountId = 0;

        Authentication authentication = await (from auth in db.Authentications
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
                Authorization authorization = await (from auth in db.Authorizations
                                                     where auth.Id == authentication.AuthorizationId.Value
                                                     select auth).FirstOrDefaultAsync();

                accountId = authorization?.AccountId ?? 0;
            }
            #endregion

            #region Using FirstStep
            if (authentication.FirstStepId.HasValue)
            {
                FirstStep firstStep = await (from fs in db.FirstSteps
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

        return account;
    }

    /// <summary>
    /// Generate Tokens with specific length
    /// </summary>
    /// <param name="size">Token Size</param>
    /// <param name="lower">Use lowercase characters.</param>
    /// <param name="upper">Use uppercase characters.</param>
    /// <returns>New token with size value.</returns>
    [NonAction]
    public static string GenerateToken(int size, bool upper = true, bool lower = true)
    {
        // ASCII characters rangers
        byte[] lowers = new byte[] { 97, 123 };
        // Upercase latters
        byte[] uppers = new byte[] { 65, 91 };
        // ASCII numbers
        byte[] numbers = new byte[] { 48, 58 };

        Random random = new();
        string result = string.Empty;

        for (int i = 0; i < size; i++)
        {
            int type = random.Next(0, lower ? 3 : 2);

            byte[] possibles = type switch
            {
                1 => upper ? uppers : numbers,
                2 => lowers,
                _ => numbers
            };

            int selected = random.Next(possibles[0], possibles[1]);
            char character = (char)selected;

            result += character;
        }

        return result;
    }
}

