using Nexus.OAuth.Api.Controllers.Base;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// Controller for user Two Factor Authentication
/// </summary>
[Route("api/Authentications/TwoFactor")]
public class AuthenticationTwoFactorController : ApiController
{
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
    public async Task<IActionResult> SendTFAAsync([FromHeader(Name = AuthorizationHeader)] string authorization,
        [FromHeader(Name = ClientKeyHeader)] string client_key,
        [FromQuery] TwoFactorType type)
    
    {
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
        throw new NotImplementedException();
    }
}