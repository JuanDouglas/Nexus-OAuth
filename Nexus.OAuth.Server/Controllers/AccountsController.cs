using Nexus.OAuth.Server.Controllers.Base;

namespace Nexus.OAuth.Server.Controllers;

/// <summary>
/// Accounts Controller
/// </summary>
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
    public async Task<IActionResult> MyAccountAsync()
    {
        Account account = ClientAccount;

        if (account == null)
            return Unauthorized();

        AccountResult result = new(account);

        return Ok(result);
    }

    /// <summary>
    /// Confirm account using method (validationType method).
    /// </summary>
    /// <param name="type">Type of method for validation.</param>
    /// <param name="code">Validation code.</param>
    /// <returns></returns>
    [HttpPost]
    [Route("Confirm")]
    public async Task<IActionResult> ValidAccountAsync(ValidationType type, [FromHeader(Name = ClientKeyHeader)] string clientKey, string code)
    {
        throw new NotImplementedException();
    }
}

