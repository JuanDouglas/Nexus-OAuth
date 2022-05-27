using Nexus.OAuth.Api.Controllers.Base;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// Teapot Controller
/// </summary>
[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
public class TeapotController : ApiController
{
    public TeapotController(IConfiguration configuration) : base(configuration)
    {
    }

    [HttpGet]
    [Route("MakeCoffe")]
    [ProducesResponseType(418)]
    public ActionResult MakeACoffe()
    {
        return StatusCode(418, "It is not possible to make coffee in a teapot.");
    }
}