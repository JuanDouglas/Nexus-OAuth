using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Linq;
using Nexus.OAuth.Api.Properties;
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
    /// Send TFA notification or message for user account
    /// </summary>
    /// <param name="type">Type of TFA method</param>
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

        if (authentication == null ||
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="code"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Confirm")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    public async Task<IActionResult> ValidTfaAsync(string code, TwoFactorType type)
    {
        TokenType tokenType;
        string[] tokens;
        string clientKey;

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

        if (authentication == null ||
            !GeneralHelpers.ValidPassword(tokens[1], authentication.Token) ||
            !GeneralHelpers.ValidPassword(clientKey, authentication.ClientKey))
            return NotFound();

        Authentication auth = authentication.authe;

        if ((auth.Date + TimeSpan.FromMilliseconds(FirsStepMaxTime * 2)) < DateTime.UtcNow)
            return NotFound();

        MongoContext mongoCtx = new(ntConn);

        TwoFactor tfa = await mongoCtx.GetTwoFactorAsync(auth.Id);

        if (!tfa.Code.Equals(code.Trim()))
            return Unauthorized();

        auth = await (from aut in db.Authentications
                      where aut.Id == auth.Id
                      select aut).FirstOrDefaultAsync();

        auth.AwaitTFA = false;

        await db.SaveChangesAsync();

        Account account = await AuthenticationHelper.GetAccountAsync(tokenType, tokens[0], db);

        await SendSecurityNotificationAsync(ntConn, account, auth);

        return Ok();
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

        Notifications.Culture = new(acc.Culture);

        switch (type)
        {
            case TwoFactorType.Email:
                await EmailSender.SendEmailAsync(code, Notifications.TitleTwoFactor, acc.Email, From);
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