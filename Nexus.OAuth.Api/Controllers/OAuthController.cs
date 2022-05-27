using Nexus.OAuth.Api.Controllers.Base;
using Nexus.Tools.Validations.Resources;
using System.Web;
using Authorization = Nexus.OAuth.Dal.Models.Authorization;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// 
/// </summary>   
[RequireAuthentication(RequireAccountValidation = false, RequiresToBeOwner = false, MinAuthenticationLevel = (int)Scope.Full)]
public class OAuthController : ApiController
{
    private const string ScopesInvalidError = "Invalid Scopes string";
    private const double AuthorizationExpires =
#if DEBUG || LOCAL
        0;
#else
      230400; // 160 days of minutes Time
#endif
    private const int CodeTokenLength = 16;
    private const double MaxCodeUseTime = 180; // Seconds Time
    public const double ExpiresAuthentication = AuthenticationsController.ExpiresAuthentication;
    private const int MinKeyLength = AuthenticationsController.MinKeyLength;
    private const int MaxKeyLength = AuthenticationsController.MaxKeyLength;

    public OAuthController(IConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Authorize OAuth App
    /// </summary>
    /// <param name="client_id">Application Key</param>
    /// <param name="scopesString">Scope of Authorization example "user,account".</param>
    /// <param name="state">Optional state token</param>
    /// <returns></returns>
    [HttpGet]
    [Route("Authorize")]
    [ProducesResponseType((int)HttpStatusCode.Redirect)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> AuthorizeAsync([FromQuery] string client_id, [FromQuery(Name = "scopes")] string scopesString, [FromQuery] string? state, bool redirect = true)
    {
        #region Valid inputs
        if (string.IsNullOrEmpty(client_id))
            ModelState.AddModelError("client_id", Errors.RequiredValidation);

        scopesString ??= string.Empty;
        string[] scopesArray = scopesString.Split(',');

        if (scopesArray.Length is < 1 or > 15)
            ModelState.AddModelError("scopes", ScopesInvalidError);

        bool validScopes = GetScopes(out Scope[] scopes, scopesString);

        if (!validScopes)
            ModelState.AddModelError("scopes", ScopesInvalidError);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        #endregion

        Account account = ClientAccount;

        Application? application = await (from app in db.Applications
                                          where app.Status > ApplicationStatus.Disabled &&
                                                app.Key == client_id
                                          select app).FirstOrDefaultAsync();

        if (application == null)
            return NotFound();

        UriBuilder uri = new(application.RedirectAuthorize);
        var query = HttpUtility.ParseQueryString(uri.Query);

        if (account.ConfirmationStatus < (application.MinConfirmationStatus ?? ConfirmationStatus.NotValided))
        {
            query["error"] = Enum.GetName(ErrorTypes.NoMinimumConfirmationStatus);
            query["error_description"] = "The account does not have the minimum value of validity required for the authorization";

            uri.Query = query.ToString();

            return Redirect(uri.ToString());
        }

        Authorization authorization = new()
        {
            AccountId = account.Id,
            ApplicationId = application.Id,
            Date = DateTime.UtcNow,
            ExpiresIn = (AuthorizationExpires != 0) ? AuthorizationExpires : null,
            Scopes = scopes,
            State = state,
            IsValid = true,
            Used = false,
            Code = GeneralHelpers.GenerateToken(CodeTokenLength)
        };

        await db.Authorizations.AddAsync(authorization);
        await db.SaveChangesAsync();

        if (!redirect)
            return Ok(new AuthorizeResult(authorization, application));

        query.Add("code", authorization.Code);
        query.Add("state", authorization.State);

        uri.Query = query.ToString();

        return Redirect(uri.ToString());
    }

    [HttpPost]
    [Route("Revoke")]
    public async Task<IActionResult> RevokeAsync(string client_id)
    {
        Account account = ClientAccount;

        Authorization? authorization = await (from auth in db.Authorizations
                                              join app in db.Applications on auth.ApplicationId equals app.Id
                                              where auth.AccountId == account.Id &&
                                                    app.Key == client_id &&
                                                    auth.IsValid
                                              select auth).FirstOrDefaultAsync();

        if (authorization == null)
            return NotFound();

        authorization.IsValid = false;

        await db.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Get access token 
    /// </summary>
    /// <param name="clientKey"></param>
    /// <param name="code"></param>
    /// <param name="client_id"></param>
    /// <param name="client_secret"></param>
    /// <param name="refresh_token"></param>
    /// <param name="token_type">Access token type</param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("AccessToken")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AuthenticationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AccessTokenAsync([FromHeader(Name = ClientKeyHeader)] string clientKey, string code, string client_id, string client_secret, string? refresh_token, TokenType token_type = TokenType.Barear)
    {
        if (string.IsNullOrEmpty(clientKey))
            return BadRequest();

        if (clientKey.Length is < MinKeyLength or > MaxKeyLength)
            return BadRequest();

        Application? application = await (from app in db.Applications
                                          where app.Key == client_id &&
                                                app.Secret == client_secret
                                          select app).FirstOrDefaultAsync();
        if (application == null)
            return Unauthorized();

        Authorization? authorization = await (from auth in db.Authorizations
                                              where auth.ApplicationId == application.Id &&
                                                    auth.Code == code &&
                                                   !auth.Used
                                              select auth).FirstOrDefaultAsync();

        if (authorization == null)
            return Unauthorized();

        if ((DateTime.UtcNow - authorization.Date).TotalSeconds > MaxCodeUseTime)
        {
            authorization.IsValid = false;
            await db.SaveChangesAsync();
            return Unauthorized();
        }

        string rfToken = GeneralHelpers.GenerateToken(AuthenticationsController.RefreshTokenSize);
        Authentication authentication = new()
        {
            IsValid = true,
            Date = DateTime.UtcNow,
            AuthorizationId = authorization.Id,
            Ip = RemoteIpAdress?.MapToIPv6().GetAddressBytes() ?? Array.Empty<byte>(),
            TokenType = token_type,
            ExpiresIn = (ExpiresAuthentication == 0) ? null : ExpiresAuthentication,
            Token = GeneralHelpers.GenerateToken(AuthenticationsController.AuthenticationTokenSize),
            RefreshToken = GeneralHelpers.HashPassword(rfToken)
        };

        authorization.Used = true;
        authorization.ClientKey = GeneralHelpers.HashPassword(clientKey);

        await db.Authentications.AddAsync(authentication);
        await db.SaveChangesAsync();

        AuthenticationResult result = new(authentication, rfToken);
        return Ok(result);
    }

    [NonAction]
    private static bool GetScopes(out Scope[] scopes, string str)
    {
        string[] scopesStrings = str.Split(',');
        scopes = Array.Empty<Scope>();

        List<Scope> scopesList = new();
        foreach (string strScope in scopesStrings)
        {
            bool isValid = Enum.TryParse(strScope, true, out Scope scope);

            if (!isValid ||
                scopesList.Contains(scope))
                return false;

            scopesList.Add(scope);
        }

        scopes = scopesList.ToArray();
        return true;
    }
}

