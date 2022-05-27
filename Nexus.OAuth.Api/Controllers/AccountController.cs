using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Api.Properties;
using System.Web;

namespace Nexus.OAuth.Api.Controllers;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Derefence of a possible null reference.

/// <summary>
/// Accounts Controller
/// </summary>
[RequireAuthentication(RequireAccountValidation = false, RequiresToBeOwner = false, MinAuthenticationLevel = (int)Scope.User)]
public class AccountController : ApiController
{
    private const string redirectSufix = "--redirect--";
    private const double minConfirmationPeriod = 1800000;
    private const int maxConfirmationsForPeriod = 5;
    private const double maxConfirmationPeriod = minConfirmationPeriod * 2;
    public AccountController(IConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Register a new account.
    /// </summary>
    /// <param name="account">Register Account Model</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPut, Route("Register")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] AccountUpload account)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Account dbAccount = account.ToDataModel();

        await db.Accounts.AddAsync(dbAccount);
        await db.SaveChangesAsync();

        AccountResult result = new(dbAccount);

        return Ok(result);
    }

    /// <summary>
    /// Get Client Account informations
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("MyAccount")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public IActionResult MyAccount()
    {
        Account? account = ClientAccount;
        AccountResult result = new(account);
        return Ok(result);
    }

    /// <summary>
    /// Send specific confirmation.
    /// </summary>
    /// <param name="type">Type of confirmation.</param>
    /// <returns></returns>
    [HttpPost, Route("SendConfirmation")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
    [RequireAuthentication(RequireAccountValidation = false, RequiresToBeOwner = true)]
    public async Task<IActionResult> SendConfirmationAsync(ConfirmationType type)
    {
        Account? account = ClientAccount;

        if ((sbyte)type < (sbyte)(account?.ConfirmationStatus ?? ConfirmationStatus.NotValided))
            return Conflict();

        DateTime minDate = DateTime.UtcNow - TimeSpan.FromMilliseconds(minConfirmationPeriod);
        int confirmations = await (from accConfirm in db.AccountConfirmations
                                   where accConfirm.AccountId == account.Id &&
                                         accConfirm.Date > minDate &&
                                         accConfirm.Type == type
                                   select accConfirm).CountAsync();

        if (confirmations >= maxConfirmationsForPeriod)
            return StatusCode((int)HttpStatusCode.TooManyRequests);

        AccountConfirmation confirmation = new()
        {
            AccountId = account.Id,
            Type = type,
            Date = DateTime.UtcNow,
            Valid = true
        };

        try
        {
            switch (type)
            {
                case ConfirmationType.PhoneNumber:
                    confirmation.Token = GeneralHelpers.GenerateToken(6, false, false);
                    break;
                case ConfirmationType.EmailAdress:
                    confirmation.Token = GeneralHelpers.GenerateToken(96);
                    string path = Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
                    string htmlContent = Resources.confirm_account;

                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query["token"] = confirmation.Token;
                    query["type"] = Enum.GetName(type);

                    string redirect = $"{WebHost}Account/Confirm?{query}";

                    htmlContent = htmlContent.Replace(redirectSufix, redirect);
                    await EmailSender.SendEmailAsync(htmlContent, "Account verification", account.Email);
                    break;
            }
        }
        catch (Exception)
        {
            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        db.AccountConfirmations.Add(confirmation);
        await db.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Confirm account using method (validationType method).
    /// </summary>
    /// <param name="type">Type of method for validation.</param>
    /// <param name="token">Validation code.</param>
    /// <returns></returns>
    [HttpPost, Route("Confirm")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [RequireAuthentication(RequireAccountValidation = false, RequiresToBeOwner = true)]
    public async Task<IActionResult> ValidAccountAsync(ConfirmationType type, string token)
    {
        Account? account = ClientAccount;

        if ((sbyte)type < (sbyte)(account?.ConfirmationStatus ?? ConfirmationStatus.NotValided))
            return Conflict();

        AccountConfirmation? confirmation = await (from accConfirm in db.AccountConfirmations
                                                   where accConfirm.Valid &&
                                                         accConfirm.AccountId == account.Id &&
                                                         accConfirm.Type == type &&
                                                         accConfirm.Token == token
                                                   select accConfirm).FirstOrDefaultAsync();
        if (confirmation == null)
            return NotFound();

        DateTime maxDate = DateTime.UtcNow - TimeSpan.FromMilliseconds(maxConfirmationPeriod);

        if (confirmation.Date > maxDate)
        {
            confirmation.Valid = false;
            await db.SaveChangesAsync();
            return NotFound();
        }

        account = await (from acc in db.Accounts
                         where acc.Id == account.Id
                         select acc).FirstOrDefaultAsync();

        account.ConfirmationStatus = (ConfirmationStatus)(type + 1);
        confirmation.Valid = false;

        await db.SaveChangesAsync();

        return Ok();
    }

    [HttpPost]
    [Route("Update")]
    [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
    public async Task<IActionResult> UpdateAsync([FromBody] Account model, [FromQuery] int id)
    {
        return StatusCode((int)HttpStatusCode.NotImplemented);
    }
}