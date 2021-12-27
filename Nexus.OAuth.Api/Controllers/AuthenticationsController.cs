using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Api.Models.Enums;
using Nexus.OAuth.Domain;
using Nexus.OAuth.Domain.Authentication;
using Nexus.OAuth.Domain.Authentication.Exceptions;
using QRCoder;
using System.Drawing;
using System.Web;

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
    public const double MaxQrCodeAge = FirsStepMaxTime;
    public const int MinKeyLength = 32;
    public const int MaxPixeisPerModuleQrCode = 150;
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
        try
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
                IpAdress = RemoteIpAdress?.ToString() ?? string.Empty
            };

            await db.FirstSteps.AddAsync(firstStep);
            await db.SaveChangesAsync();

            FirstStepResult result = new(firstStep, firsStepToken, FirsStepMaxTime);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return Ok(ex);
        }
    }

    [HttpOptions]
    [AllowAnonymous]
    [Route("SetCookie")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> SetCookie()
    {
        try
        {
            (TokenType tokenType, string firstToken, string secondToken, string clientKey) = AuthenticationHelper.GetAuthorization(HttpContext);

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

        if (!GeneralHelpers.ValidPassword(pwd, account?.Password ?? string.Empty))
            return Unauthorized();

        string gntToken = GeneralHelpers.GenerateToken(AuthenticationTokenSize);
        string rfToken = GeneralHelpers.GenerateToken(RefreshTokenSize);
        Authentication authentication = new()
        {
            Date = DateTime.UtcNow,
            FirstStepId = firstStep.Id,
            IsValid = true,
            Token = gntToken,
            RefreshToken = GeneralHelpers.HashPassword(rfToken),
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
    #region QrCode
    /// <summary>
    /// 
    /// </summary>
    /// <param name="client_key">Client unique key</param>
    /// <param name="user_agent">Client user agent</param>
    /// <param name="pixeis_per_module">Pixeis per module (<c>Default: 5</c>, <c>Max: 50</c>)</param>
    /// <returns></returns>
    [HttpGet]
    [Route("QrCode/Generate")]
    [ProducesResponseType(typeof(byte[]), (int)HttpStatusCode.OK, "image/png")]
    public async Task<IActionResult> GetQrCodeAsync([FromHeader(Name = ClientKeyHeader)] string client_key, [FromHeader(Name = UserAgentHeader)] string user_agent, Theme theme = Theme.Dark, bool transparent = true, int? pixeis_per_module = 5)
    {
        if (string.IsNullOrEmpty(client_key) ||
            string.IsNullOrEmpty(user_agent))
            return BadRequest();

        if (client_key.Length < MinKeyLength ||
            client_key.Length > 256)
            return BadRequest();

        if (pixeis_per_module > MaxPixeisPerModuleQrCode)
            pixeis_per_module = MaxPixeisPerModuleQrCode;

        string code = GeneralHelpers.GenerateToken(9, false, false);
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["code"] = code;
        query["registor_key"] = client_key;

        UriBuilder uri = new($"https://{Request.Host}");
        uri.Path = "api/Authentications/QrCode/Authorize";
        uri.Query = query.ToString();
        string url = uri.ToString();
        string validationToken = GeneralHelpers.GenerateToken(AuthenticationTokenSize, false);

        QrCodeReference qrCodeReference = new()
        {
            ClientKey = GeneralHelpers.HashPassword(client_key),
            Code = code,
            Create = DateTime.UtcNow,
            Valid = true,
            IpAdress = RemoteIpAdress?.ToString() ?? string.Empty,
            UserAgent = user_agent,
            ValidationToken = GeneralHelpers.HashPassword(validationToken)
        };

        QRCodeGenerator qrGenerator = new();
        QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);

        var qrCode = new PngByteQRCode(qrCodeData);
        byte[] bytes = qrCode.GetGraphic(pixeis_per_module ?? 5,
            GetPrimaryColor(theme),
            new byte[] { 255, 255, 255, (byte)(transparent ? 0 : 255) });

        await db.QrCodes.AddAsync(qrCodeReference);
        await db.SaveChangesAsync();

        Response.Headers["X-Code"] = code;
        Response.Headers["X-Validation"] = validationToken;

        return File(bytes, "image/png");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="registor_key">Client key of registor</param>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("QrCode/Authorize")]
    [RequireAuthentication(ShowView = true)]
    public async Task<IActionResult> AuthorizeCodeAsync(string registor_key, string code)
    {
        QrCodeReference? codeReference = await (from qrCode in db.QrCodes
                                                where qrCode.Code == code &&
                                                      qrCode.Valid &&
                                                      !qrCode.Used
                                                select qrCode).FirstOrDefaultAsync();
        if (codeReference == null)
            return NotFound();

        if ((DateTime.UtcNow - codeReference.Create).TotalMilliseconds > MaxQrCodeAge)
        {
            codeReference.Valid = false;
            await db.SaveChangesAsync();
        }

        if (!GeneralHelpers.ValidPassword(registor_key, codeReference.ClientKey))
            return NotFound();

        codeReference.Used = true;
        codeReference.Valid = false;
        codeReference.Use = DateTime.UtcNow;


        throw new NotImplementedException();
    }
    #endregion
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
            (tokenType, firstToken, secondToken, clientKey) = AuthenticationHelper.GetAuthorization(HttpContext);
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

        if (!GeneralHelpers.ValidPassword(refresh_token, authentication?.RefreshToken ?? string.Empty))
        {

        }

        throw new NotImplementedException();
    }

    private static byte[] GetPrimaryColor(Theme theme) => theme switch
    {
        Theme.Light => new byte[] { 190, 190, 190 },
        _ => new byte[3]
    };
}