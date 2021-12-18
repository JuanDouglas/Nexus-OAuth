using Nexus.OAuth.Server.Controllers.Base;
using Nexus.Tools.Validations.Middlewares.Authentication;

namespace Nexus.OAuth.Server.Controllers;

/// <summary>
/// 
/// </summary>
[AllowAnonymous]
public class AuthenticationsController : ApiController
{
    public const string AuthorizationTokenName = "Authorization";
    public const int FirstTokenSize = 32;
    public const int AuthenticationTokenSize = 96;
    public const double FirsStepMaxTime = 600000; // Milisecond time
    public const int MinKeyLength = 32;
    public const double
#if DEBUG 
        ExpiresAuthentication = 0;
#else
        ExpiresAuthentication = 0; // Minutes time
#endif



    /// <summary>
    /// Get FirstStep token for authentication.
    /// </summary>
    /// <param name="user">User e-mail</param>
    /// <param name="client_key">Unique hash key for client (Min 32 length).</param>
    /// <param name="redirect">After proccess redirect url</param>
    /// <returns></returns>
    [HttpGet]
    [Route("FirstStep")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(FirstStepResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> FirstStepAsync(string user, string client_key, string? redirect)
    {
        if (string.IsNullOrEmpty(user) ||
            string.IsNullOrEmpty(client_key))
            return BadRequest();

        if (client_key.Length < MinKeyLength)
            return BadRequest();

        Account account = await (from fs in db.Accounts
                                 where fs.Email == user
                                 select fs).FirstOrDefaultAsync();

        if (account == null)
            return NotFound();

        // No verify complex token 
        string firsStepToken = GenerateToken(FirstTokenSize, lower: false);

        FirstStep firstStep = new()
        {
            Key = HashPassword(client_key),
            IsValid = true,
            Date = DateTime.UtcNow,
            AccountId = account.Id,
#warning Update database
            Redirect = redirect ?? string.Empty,
            Token = HashPassword(firsStepToken),
            IpAdress = RemoteIpAdress?.ToString() ?? string.Empty
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
    /// <param name="client_key">Unique Client Key</param>
    /// <param name="token">First Step token</param>
    /// <param name="fs_id">First Step id</param>
    /// <returns></returns>
    [HttpGet]
    [Route("SecondStep")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AuthenticationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> SecondStepAsync(string pwd, string client_key, string token, int fs_id)
    {
        if (string.IsNullOrEmpty(UserAgent))
            return BadRequest();

        FirstStep firstStep = await (from fs in db.FirstSteps
                                     where fs.Id == fs_id &&
                                          fs.IsValid
                                     select fs).FirstOrDefaultAsync();

        if (firstStep == null)
            return Unauthorized();

        if (!ValidPassword(token, firstStep.Token) ||
            !ValidPassword(client_key, firstStep.Key) ||
            string.IsNullOrEmpty(pwd))
            return Unauthorized();

        if ((DateTime.UtcNow - firstStep.Date).Milliseconds >= FirsStepMaxTime)
        {
            firstStep.IsValid = false;

            await db.SaveChangesAsync();

            return Unauthorized();
        }

        Account account = await (from fs in db.Accounts
                                 where fs.Id == firstStep.AccountId
                                 select fs).FirstOrDefaultAsync();

        if (!ValidPassword(pwd, account.Password))
            return Unauthorized();

        string gntToken = GenerateToken(AuthenticationTokenSize);
        string rfToken = GenerateToken(AuthenticationTokenSize);
        Authentication authentication = new()
        {
            Date = DateTime.UtcNow,
            FirstStepId = firstStep.Id,
            IsValid = true,
            Token = gntToken,
            RefreshToken = rfToken,
            ExpiresIn = (ExpiresAuthentication == 0) ? null : ExpiresAuthentication,
            IpAdress = RemoteIpAdress?.ToString() ?? string.Empty
        };

        firstStep.IsValid = false;

        await db.Authentications.AddAsync(authentication);
        await db.SaveChangesAsync();

        AuthenticationResult result = new(authentication);

        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    [NonAction]
    public static async Task<AuthenticationMidddleware.AuthenticationResult> ValidAuthenticationResultAsync(HttpContext ctx)
    {
        throw new NotImplementedException();
    }
}