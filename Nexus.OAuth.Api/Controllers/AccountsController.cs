using Nexus.OAuth.Api.Controllers.Base;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// Accounts Controller
/// </summary>
[RequireAuthentication(RequireAccountValidation = false, ShowView = true)]
public class AccountsController : ApiController
{
    /// <summary>
    /// Register a new account.
    /// </summary>
    /// <param name="account">Register Account Model</param>
    /// <returns></returns>
    [HttpPut]
    [AllowAnonymous]
    [Route("Register")]
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

    [HttpGet]
    [Route("MyAccount")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public IActionResult MyAccount() =>
#pragma warning disable CS8604 // Possible null reference argument.
        Ok(new AccountResult(ClientAccount));
#pragma warning restore CS8604 

    [HttpPost]
    [Route("SendConfirmation")]
    public async Task<IActionResult> SendConfirmationAsync(ConfirmationType type)
    {
        Account? account = ClientAccount;

        if ((sbyte)type < (sbyte)(account?.ConfirmationStatus ?? ConfirmationStatus.NotValided))
            return Conflict();

        switch (type)
        {
            case ConfirmationType.PhoneNumber:
                break;
            case ConfirmationType.EmailAdress:
                break;
        }

        return Ok();
    }

    /// <summary>
    /// Confirm account using method (validationType method).
    /// </summary>
    /// <param name="type">Type of method for validation.</param>
    /// <param name="code">Validation code.</param>
    /// <returns></returns>
    [HttpPost]
    [Route("Confirm")]
    public async Task<IActionResult> ValidAccountAsync(ConfirmationType type, [FromHeader(Name = ClientKeyHeader)] string clientKey, string code)
    {
        throw new NotImplementedException();
    }
}

