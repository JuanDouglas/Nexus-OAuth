using BenjaminAbt.HCaptcha;
using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Api.Properties;
using Nexus.OAuth.Domain.Authentication;
using Nexus.OAuth.Domain.Authentication.Exceptions;
using System.Globalization;
using System.Web;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// 
/// </summary>
[AllowAnonymous]
public class AuthenticationsController : Base.AuthenticationsController
{
    public const int FirstTokenSize = 32;
    public const double ExpiresAuthentication = 0; // Minutes time

    /// <summary>
    /// 
    /// </summary>
    /// <param name="captchaValidator"></param>
    /// <param name="config"></param>
    public AuthenticationsController(IHCaptchaApi captchaValidator, IConfiguration config)
        : base(captchaValidator, config)
    {

    }

    /// <summary>
    /// Get FirstStep token for authentication.
    /// </summary>
    /// <param name="user">User e-mail</param>
    /// <param name="client_key">Unique hash key for client (Min 32 length).</param>
    /// <param name="redirect">After proccess redirect url</param>
    /// <param name="hCaptchaToken">Validation captcha token</param>
    /// <param name="userAgent"> Client user Agent header</param>
    /// <returns></returns>
    [HttpGet]
    [Route("FirstStep")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(FirstStepResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> FirstStepAsync([FromHeader(Name = UserAgentHeader)] string userAgent, [FromHeader(Name = ClientKeyHeader)] string client_key, string user, string? hCaptchaToken, string? redirect, bool noContent = false)
    {
        if (string.IsNullOrEmpty(user) ||
            string.IsNullOrEmpty(client_key) ||
            string.IsNullOrEmpty(userAgent))
            return BadRequest();

        if (client_key.Length < MinKeyLength ||
            client_key.Length > MaxKeyLength)
            return BadRequest();

        if (ModelState.IsValid &&
            !Program.IsDebug)
        {
            var captchaResp = await captchaValidator.Verify(hCaptchaKey, hCaptchaToken ?? string.Empty);

            if (!(captchaResp?.Success ?? false))
                return StatusCode((int)HttpStatusCode.Unauthorized);
        }

        Account? account = await (from fs in db.Accounts
                                  where fs.Email == HttpUtility.UrlDecode(user)
                                  select fs).FirstOrDefaultAsync();

        if (account == null)
            return NotFound();

        if (noContent)
            return StatusCode((int)HttpStatusCode.NoContent);

        if (account.ProfileImageID != null && account.ProfileImage == null)
            account.ProfileImage = await (from fl in db.Files
                                          where fl.Id == account.ProfileImageID
                                          select fl).FirstOrDefaultAsync();


        // No verify complex token 
        string firsStepToken = GeneralHelpers.GenerateToken(FirstTokenSize, lower: false);

        FirstStep firstStep = new()
        {
            ClientKey = GeneralHelpers.HashPassword(client_key),
            IsValid = true,
            Date = DateTime.UtcNow,
            AccountId = account.Id,
            Redirect = redirect,
            UserAgent = userAgent,
            Token = GeneralHelpers.HashPassword(firsStepToken),
            Ip = RemoteIpAdress?.MapToIPv6().GetAddressBytes() ?? Array.Empty<byte>()
        };

        await db.FirstSteps.AddAsync(firstStep);
        await db.SaveChangesAsync();

        FirstStepResult result = new(account, firstStep, firsStepToken, FirsStepMaxTime);

        await SendTryLoginNotificationAsync(ntConn, account, firstStep);

        return Ok(result);
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
    [ProducesResponseType(typeof(AuthenticationResult), (int)HttpStatusCode.Continue)]
    public async Task<IActionResult> SecondStepAsync(string pwd, string token, int fs_id, [FromHeader(Name = ClientKeyHeader)] string client_key, TokenType tokenType = TokenType.Barear)
    {
        FirstStep? firstStep = await (from fs in db.FirstSteps
                                      where fs.Id == fs_id &&
                                           fs.IsValid
                                      select fs).FirstOrDefaultAsync();
        if (firstStep == null)
            return Unauthorized();

        if (!GeneralHelpers.ValidPassword(token, firstStep.Token) ||
            !GeneralHelpers.ValidPassword(client_key, firstStep.ClientKey) ||
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

        if (!GeneralHelpers.ValidPassword(HttpUtility.UrlDecode(pwd), account?.Password ?? string.Empty))
            return Unauthorized();

        string rfToken = GeneralHelpers.GenerateToken(RefreshTokenSize);
        Authentication authentication = new()
        {
            Date = DateTime.UtcNow,
            FirstStepId = firstStep.Id,
            IsValid = true,
            AwaitTFA = account.TFAEnable,
            Token = GeneralHelpers.GenerateToken(AuthenticationTokenSize),
            RefreshToken = GeneralHelpers.HashPassword(rfToken),
            TokenType = tokenType,
            ExpiresIn = (ExpiresAuthentication == 0) ? null : ExpiresAuthentication,
            Ip = RemoteIpAdress?.MapToIPv6().GetAddressBytes() ?? Array.Empty<byte>()
        };

        firstStep.IsValid = false;

        await db.Authentications.AddAsync(authentication);
        await db.SaveChangesAsync();

        AuthenticationResult result = new(authentication, rfToken);

        if (account.TFAEnable)
            return StatusCode((int)HttpStatusCode.IMUsed, result);

        await SendSecurityNotificationAsync(ntConn, account, authentication);

        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpOptions]
    [AllowAnonymous]
    [Route("SetCookie")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public IActionResult SetCookie()
    {
        try
        {
            (TokenType tokenType, string[] tokens, string clientKey) = AuthenticationHelper.GetAuthorization(HttpContext);

            string authorization = string.Empty;
            switch (tokenType)
            {
                case TokenType.Barear:
                    authorization = $"{tokenType} {string.Join('.', tokens)}";
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
                Secure = true,
                SameSite = SameSiteMode.None
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
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("Refresh")]
    [ProducesResponseType(typeof(AuthenticationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RefreshTokenAsync(string refresh_token, [FromHeader(Name = AuthorizationHeader)] string authorization, [FromHeader(Name = ClientKeyHeader)] string clientKey)
    {
        TokenType tokenType;
        string[] tokens;

        try
        {
            (tokenType, tokens, clientKey) = AuthenticationHelper.GetAuthorization(HttpContext);
        }
        catch (AuthenticationException ex)
        {
            return BadRequest(new
            {
                Error = ex.Message,
                In = ex.Header
            });
        }

        string token = (tokens.Length >= 1) ? tokens.FirstOrDefault() : string.Empty;

        Authentication? authentication = await (from auth in db.Authentications
                                                join fs in db.FirstSteps on auth.FirstStepId equals fs.Id
                                                where !auth.IsValid &&
                                                     auth.Token == token
                                                select auth).FirstOrDefaultAsync();

        if (!GeneralHelpers.ValidPassword(refresh_token, authentication?.RefreshToken ?? string.Empty))
        {

        }

        throw new NotImplementedException();
    }

    /// <summary>
    /// Logout your login.
    /// </summary>
    /// <returns>Ok response</returns>
    [HttpPost]
    [Route("Logout")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> LogoutAsync()
    {
        try
        {
            (TokenType tokenType, string[] tokens, _) = AuthenticationHelper.GetAuthorization(HttpContext);

            Account? account = await AuthenticationHelper.GetAccountAsync(tokenType, tokens[0], db);

            if (account == null)
                return Unauthorized();

            Authentication? authentication = await (from auth in db.Authentications
                                                    where auth.Token == tokens[0]
                                                    select auth).FirstOrDefaultAsync();
            if (authentication != null)
            {
                authentication.IsValid = false;

                db.Entry(authentication).State = EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }
        catch (AuthenticationException)
        {
            return Unauthorized();
        }

        return Ok();
    }
}