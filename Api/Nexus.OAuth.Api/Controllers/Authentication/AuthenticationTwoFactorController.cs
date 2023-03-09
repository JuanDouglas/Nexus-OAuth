using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Domain.Authentication;
using Nexus.OAuth.Domain.Authentication.Exceptions;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// Controller for user Two Factor Authentication
/// </summary>
[Route("api/Authentications/TwoFactor")]
public class AuthenticationTwoFactorController : Base.AuthenticationsController
{
    private const int TwoFactorLength = 6;

    public AuthenticationTwoFactorController(IConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("Send")]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> SendTFAAsync([FromQuery] TwoFactorType type)
    {
        TokenType tokenType;
        string[] tokens;
        string clientKey, code;

        try
        {
            (tokenType, tokens, clientKey) = AuthenticationHelper.GetAuthorization(Request.HttpContext);
        }
        catch (AuthenticationException)
        {
            return BadRequest();
        }

        var authentication = await (from authe in db.Authentications
                                    where authe.TokenType == tokenType &&
                                          authe.Token == tokens[0] &&
                                          authe.IsValid &&
                                          authe.AwaitTFA
                                    select new { authe, authe.FirstStepNavigation.ClientKey, authe.FirstStepNavigation.AccountId, authe.FirstStepNavigation.Token }).FirstOrDefaultAsync();

        if (authentication == null|| 
            !GeneralHelpers.ValidPassword(tokens[1], authentication.Token) ||
            !GeneralHelpers.ValidPassword(clientKey, authentication.ClientKey))
            return NotFound();

        Authentication auth = authentication.authe;

        if (auth.ExpiresIn.HasValue)
        {
            if (auth.ExpiresIn > 0 &&
                (DateTime.UtcNow - auth.Date).TotalSeconds > auth.ExpiresIn)
            {
                auth.IsValid = false;
                await db.SaveChangesAsync();
            }
        }

        code = GeneralHelpers.GenerateToken(TwoFactorLength, false, false);

        await SendTwoFactorAsync(authentication.AccountId, code, type);

        MongoContext mongoCtx = new(ntConn);

        await mongoCtx.ApplyTwoFactorAsync(new()
        {
            Send = DateTime.UtcNow,
            Type = type,
            Code = code,
            AuthId = auth.Id
        });

        return Ok();
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("Confirm")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ValidTFAAsync([FromHeader(Name = AuthorizationHeader)] string authorization,
        [FromHeader(Name = ClientKeyHeader)] string client_key,
        [FromQuery] string code,
        [FromQuery] TwoFactorType type)
    {
        return Ok();
        // SendSecurityNotificationAsync();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="type"></param>
    private async Task SendTwoFactorAsync(int accountId, string code, TwoFactorType type)
    {
        Account acc = await (from account in db.Accounts
                             where account.Id == accountId
                             select account).FirstOrDefaultAsync();

        switch (type)
        {
            case TwoFactorType.Email:
                await EmailSender.SendEmailAsync(code, "Two Factor", acc.Email, "support@mail.nexus-company.net");
                break;
            case TwoFactorType.Phone:
                break;
            case TwoFactorType.App:
                break;
            case TwoFactorType.Wpp:
                break;
            default:
                break;
        }
    }
}