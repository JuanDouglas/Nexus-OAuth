using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Api.Exceptions;
using Nexus.Tools.Validations.Middlewares.Authentication;
using System.Collections.Specialized;
using Authorization = Nexus.OAuth.Dal.Models.Authorization;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// 
/// </summary>
[AllowAnonymous]
public class AuthenticationsController : ApiController
{
    public const int FirstTokenSize = 32;
    public const int AuthenticationTokenSize = 96;
    public const int RefreshTokenSize = 128;
    public const double FirsStepMaxTime = 600000; // Milisecond time
    public const int MinKeyLength = 32;
    public const double
#if DEBUG || LOCAL
        ExpiresAuthentication = 0;
#else
        ExpiresAuthentication = 0; // Minutes time
#endif

    /// <summary>
    /// Get FirstStep token for authentication.
    /// </summary>
    /// <param name="user">User e-mail</param>
    /// <param name="client_key">Unique hash key for client (Min 32 length).</param>
    /// <param name="redirect">After proccess redirect url</param>
    /// <returns></returns>
    [HttpGet]
    [Route("FirstStep")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(FirstStepResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> FirstStepAsync(string user, string? redirect, [FromHeader(Name = UserAgentHeader)] string userAgent, [FromHeader(Name = ClientKeyHeader)] string client_key)
    {
        if (string.IsNullOrEmpty(user) ||
            string.IsNullOrEmpty(client_key) ||
            string.IsNullOrEmpty(userAgent))
            return BadRequest();

        if (client_key.Length < MinKeyLength ||
            client_key.Length > 256)
            return BadRequest();

        Account? account = await (from fs in db.Accounts
                                  where fs.Email == user
                                  select fs).FirstOrDefaultAsync();

        if (account == null)
            return NotFound();

        // No verify complex token 
        string firsStepToken = GenerateToken(FirstTokenSize, lower: false);

        FirstStep firstStep = new()
        {
            ClientKey = HashPassword(client_key),
            IsValid = true,
            Date = DateTime.UtcNow,
            AccountId = account.Id,
            Redirect = redirect,
            UserAgent = userAgent,
            Token = HashPassword(firsStepToken),
            IpAdress = RemoteIpAdress?.ToString() ?? string.Empty
        };

        await db.FirstSteps.AddAsync(firstStep);
        await db.SaveChangesAsync();

        FirstStepResult result = new(firstStep, firsStepToken, FirsStepMaxTime);

        return Ok(result);
    }

    [HttpOptions]
    [AllowAnonymous]
    [Route("SetCookie")]
    public async Task<IActionResult> SetCookie()
    {
        try
        {
            (TokenType tokenType, string firstToken, string secondToken, string clientKey) = GetAuthorization(HttpContext);

            string authorization = string.Empty;
            switch (tokenType)
            {
                case TokenType.Barear:
                    authorization = $"{tokenType} {firstToken}.{secondToken}";
                    break;
                case TokenType.Basic:
                    break;
                default:
                    break;
            }

            CookieOptions options = new()
            {
                HttpOnly = true,
                IsEssential = true,
                MaxAge = TimeSpan.Zero,
                Secure = true,
                Domain = Request.Host.ToString(),
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append(AuthorizationHeader, authorization, options);
            Response.Cookies.Append(ClientKeyHeader, clientKey, options);
        }
        catch (AuthenticationException ex)
        {
            return BadRequest(ex.Message);
        }
        return Ok();
    }
    /// <summary>
    /// Get authorization token.
    /// </summary>
    /// <param name="pwd">Account password</param>
    /// <param name="client_key">Unique Client Key</param>
    /// <param name="token">First Step token</param>
    /// <param name="fs_id">First Step id</param>
    /// <param name="tokenType">Type of authentication token</param>
    /// <returns></returns>
    [HttpGet]
    [Route("SecondStep")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AuthenticationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SecondStepAsync(string pwd, string token, int fs_id, [FromHeader(Name = ClientKeyHeader)] string client_key, TokenType tokenType = TokenType.Barear)
    {
        FirstStep? firstStep = await (from fs in db.FirstSteps
                                      where fs.Id == fs_id &&
                                           fs.IsValid
                                      select fs).FirstOrDefaultAsync();

        if (firstStep == null)
            return Unauthorized();

        if (!ValidPassword(token, firstStep.Token) ||
            !ValidPassword(client_key, firstStep.ClientKey) ||
            string.IsNullOrEmpty(pwd))
            return Unauthorized();

        if ((DateTime.UtcNow - firstStep.Date).Milliseconds >= FirsStepMaxTime)
        {
            firstStep.IsValid = false;

            await db.SaveChangesAsync();

            return Unauthorized();
        }

        Account? account = await (from fs in db.Accounts
                                  where fs.Id == firstStep.AccountId
                                  select fs).FirstOrDefaultAsync();

        if (!ValidPassword(pwd, account?.Password ?? string.Empty))
            return Unauthorized();

        string gntToken = GenerateToken(AuthenticationTokenSize);
        string rfToken = GenerateToken(RefreshTokenSize);
        Authentication authentication = new()
        {
            Date = DateTime.UtcNow,
            FirstStepId = firstStep.Id,
            IsValid = true,
            Token = gntToken,
            RefreshToken = HashPassword(rfToken),
            TokenType = tokenType,
            ExpiresIn = (ExpiresAuthentication == 0) ? null : ExpiresAuthentication,
            IpAdress = RemoteIpAdress?.ToString() ?? string.Empty
        };

        firstStep.IsValid = false;

        await db.Authentications.AddAsync(authentication);
        await db.SaveChangesAsync();

        AuthenticationResult result = new(authentication, rfToken);

        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("Refresh")]
    [ProducesResponseType(typeof(AuthenticationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RefreshTokenAsync(string refresh_token, [FromHeader(Name = AuthorizationHeader)] string authorization, [FromHeader(Name = ClientKeyHeader)] string clientKey)
    {
        TokenType tokenType;
        string firstToken, secondToken;

        try
        {
            (tokenType, firstToken, secondToken, clientKey) = GetAuthorization(HttpContext);
        }
        catch (AuthenticationException ex)
        {
            return BadRequest(new
            {
                Error = ex.Message,
                In = ex.Header
            });
        }

        Authentication? authentication = await (from auth in db.Authentications
                                                join fs in db.FirstSteps on auth.FirstStepId equals fs.Id
                                                where !auth.IsValid &&
                                                     auth.Token == firstToken

                                                select auth).FirstOrDefaultAsync();

        if (!ValidPassword(refresh_token, authentication?.RefreshToken ?? string.Empty))
        {

        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// Valid if context authentication isValid.
    /// </summary>
    /// <param name="ctx">HttpContext for this request</param>
    /// <returns></returns>
    [NonAction]
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

        Authentication? authentication = await (from fs in db.Authentications
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
                ValidPassword(clientKey, firstStep?.ClientKey ?? string.Empty) &&
                ValidPassword(secondToken, firstStep?.Token ?? string.Empty);
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

        return new(isValid, isConfirmed);
    }

}