using Nexus.OAuth.Server.Controllers.Base;

namespace Nexus.OAuth.Server.Controllers;

public class ApplicationsController : ApiController
{
    public const int ApplicationKeyLength = 96;
    public const int ApplicationSecretLength = 256;

    /// <summary>
    /// Create a new application
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("Create")]
    public async Task<IActionResult> CreateAsync(ApplicationUpload application)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Application dbApplication = application.ToDataModel();

        await db.Applications.AddAsync(dbApplication);
        await db.SaveChangesAsync();

        ApplicationResult result = new(dbApplication);

        return Ok(result);
    }

    [HttpPost]
    [Route("List")]
    [ProducesResponseType(typeof(ApplicationResult[]), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ListAsync()
    {
        throw new NotImplementedException();
    }
}

