using Nexus.OAuth.Server.Controllers.Base;

namespace Nexus.OAuth.Server.Controllers;

/// <summary>
/// 
/// </summary>
public class AccountsController : ApiController
{
    /// <summary>
    /// Register a new account.
    /// </summary>
    /// <param name="account">Register Account Model</param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    [Route("Register")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] AccountUpload account)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Account dbAccount = account.DbModel();

        await db.Accounts.AddAsync(dbAccount);
        await db.SaveChangesAsync();

        AccountResult result = new(dbAccount);

        return Ok(result);
    }

    /// <summary>
    /// 
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
    /// Valid account
    /// </summary>
    /// <param name="type"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("Confirm")]
    public async Task<IActionResult> ValidAccountAsync(ValidationType type, [FromHeader(Name = ClientKeyHeader)] string clientKey, string code)
    {
        throw new NotImplementedException();
    }
}

