using Nexus.OAuth.Server.Controllers.Base;

namespace Nexus.OAuth.Server.Controllers;

/// <summary>
/// 
/// </summary>
public class AuthenticationController : ApiController
{
    public const string AuthorizationTokenName = "Authorization";
    public const int FirstTokenSize = 72;
    public const double FirsStepMaxTime = 600000; //Milisecond time
    public const int MinKeyLength = 32;

    /// <summary>
    /// Get FirstStep token for authentication.
    /// </summary>
    /// <param name="user">User e-mail</param>
    /// <param name="key">Unique hash key for client (Min 32 length)</param>
    /// <param name="redirect">Redirect url</param>
    /// <returns></returns>
    [Route("FirstStep")]
    [ProducesDefaultResponseType(typeof(FirstStepResult))]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> FirstStepAsync(string user, string key, string redirect)
    {
        if (string.IsNullOrEmpty(user) ||
            string.IsNullOrEmpty(key))
            return BadRequest();

        if (key.Length < MinKeyLength)
            return BadRequest();

        Account account = await db.Accounts.FirstOrDefaultAsync(fs => fs.Email == user);

        if (account == null)
            return NotFound();

        string firsStepToken = GenerateToken(FirstTokenSize);

        FirstStep firstStep = new()
        {
            Key = HashPassword(key),
            IsValid = true,
            Date = DateTime.UtcNow,
            AccountId = account.Id,
            Redirect = redirect,
            Token = HashPassword(firsStepToken)
        };

        await db.FirstSteps.AddAsync(firstStep);
        await db.SaveChangesAsync();

        FirstStepResult result = new(firstStep, firsStepToken, FirsStepMaxTime);

        return Ok(result);
    }

    /// <summary>
    /// Get authorization token.
    /// </summary>
    /// <param name="pwd">Account password</param>
    /// <param name="key">Unique Client Key</param>
    /// <param name="token">First Step token</param>
    /// <param name="fsId">First Step id</param>
    /// <returns></returns>
    [Route("SecondStep")]
    public async Task<IActionResult> SecondStepAsync(string pwd, string key, string token, int fsId)
    {
        if (string.IsNullOrEmpty(UserAgent))
            return BadRequest();

        FirstStep firstStep = await db.FirstSteps.FirstOrDefaultAsync(fs => fs.Id == fsId);

        if (firstStep == null)
            return Unauthorized();

        if (!ValidPassword(token, firstStep.Token) ||
            !ValidPassword(key, firstStep.Key) ||
            string.IsNullOrEmpty(pwd))
            return Unauthorized();

        Account account = await db.Accounts.FirstOrDefaultAsync(fs => fs.Id == firstStep.AccountId);

        if (!ValidPassword(pwd, account.Password))
            return Unauthorized();




        throw new NotImplementedException();
    }
}