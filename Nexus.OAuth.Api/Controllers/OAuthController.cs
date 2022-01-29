using Nexus.OAuth.Api.Controllers.Base;
using Nexus.Tools.Validations.Resources;
using System.Web;
using Authorization = Nexus.OAuth.Dal.Models.Authorization;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// 
/// </summary>
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
    public async Task<IActionResult> AuthorizeAsync([FromQuery] string client_id, [FromQuery(Name = "scopes")] string scopesString, [FromQuery] string? state)
    {
        #region Valid inputs
        if (string.IsNullOrEmpty(client_id))
            ModelState.AddModelError("client_id", Errors.RequiredValidation);

        scopesString ??= string.Empty;
        string[] scopesArray = scopesString.Split(',');

        if (scopesArray.Length < 1 || scopesArray.Length > 15)
            ModelState.AddModelError("scopes", ScopesInvalidError);

        bool validScopes = GetScopes(out Scope[] scopes, scopesString);

        if (!validScopes)
            ModelState.AddModelError("scopes", ScopesInvalidError);

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        #endregion

        Account? account = ClientAccount;

        Application? application = await (from app in db.Applications
                                          where app.Status > ApplicationStatus.Disabled &&
                                                app.Key == client_id
                                          select app).FirstOrDefaultAsync();

        if (application == null)
            return NotFound();

        Authorization authorization = new()
        {
            AccountId = account.Id,
            ApplicationId = application.Id,
            Date = DateTime.UtcNow,
            ExpiresIn = (AuthorizationExpires != 0) ? AuthorizationExpires : null,
            Scopes = scopes,
            State = state,
            IsValid = true,
            Code = GeneralHelpers.GenerateToken(CodeTokenLength)
        };

        await db.Authorizations.AddAsync(authorization);
        await db.SaveChangesAsync();

        UriBuilder uri = new(application.RedirectAuthorize);
        var query = HttpUtility.ParseQueryString(uri.Query);

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



        return Ok();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("AccessToken")]
    public async Task<IActionResult> AccessTokenAsync(string code, string client_id, string client_secret, string? refresh_token, TokenType token_type = TokenType.Barear)
    {
        Application? application = await (from app in db.Applications
                                          where app.Key == client_id &&
                                                app.Secret == client_secret
                                          select app).FirstOrDefaultAsync();
        if (application == null)
            return Unauthorized();


        Authorization? authorization = await (from auth in db.Authorizations
                                              where auth.ApplicationId == application.Id &&
                                                    auth.Code == code &&
                                                    auth.IsValid
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
            IpAdress = RemoteIpAdress.ToString(),
            TokenType = token_type,
            ExpiresIn = (ExpiresAuthentication == 0) ? null : ExpiresAuthentication,
            Token = GeneralHelpers.GenerateToken(AuthenticationsController.AuthenticationTokenSize),
            RefreshToken = GeneralHelpers.HashPassword(rfToken)
        };

        authorization.IsValid = false;

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

