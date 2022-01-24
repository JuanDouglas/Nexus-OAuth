using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Domain.Storage;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// Accounts Controller
/// </summary>
[RequireAuthentication(RequireAccountValidation = false, ShowView = true)]
public class AccountsController : ApiController
{
    public AccountsController(IConfiguration configuration) : base(configuration)
    {
    }

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
#pragma warning disable CS8604 // Possible null reference argument.
    [HttpGet]
    [Route("MyAccount")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public IActionResult MyAccount()
    {
        Account? account = ClientAccount;
        AccountResult result = new(account);
        return Ok(result);
    }

#pragma warning restore CS8604

    /// <summary>
    /// Send specific confirmation.
    /// </summary>
    /// <param name="type">Type of confirmation.</param>
    /// <returns></returns>
    [HttpPost]
    [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
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

        return StatusCode((int)HttpStatusCode.NotImplemented);
    }

    /// <summary>
    /// Confirm account using method (validationType method).
    /// </summary>
    /// <param name="type">Type of method for validation.</param>
    /// <param name="code">Validation code.</param>
    /// <returns></returns>
    [HttpPost]
    [Route("Confirm")]

    [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
    public async Task<IActionResult> ValidAccountAsync(ConfirmationType type, [FromHeader(Name = ClientKeyHeader)] string clientKey, string code)
    {
        return StatusCode((int)HttpStatusCode.NotImplemented);
    }
}

