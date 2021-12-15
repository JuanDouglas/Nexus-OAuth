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
    /// <param name="account">Account</param>
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

    [HttpGet]
    [Route("MyAccount")]
    public async Task<IActionResult> MyAccountAsync()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Valid account
    /// </summary>
    /// <param name="type"></param>
    /// <param name="code"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("Valid")]
    public async Task<IActionResult> ValidAccountAsync(ValidationType type, string code)
    {
        throw new NotImplementedException();
    }
}

