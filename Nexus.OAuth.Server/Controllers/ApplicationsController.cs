using Nexus.OAuth.Server.Controllers.Base;

namespace Nexus.OAuth.Server.Controllers;

public class ApplicationsController : ApiController
{
    public const int ApplicationKeyLength = 32;
    public const int ApplicationSecretLength = 96;

    /// <summary>
    /// Create a new application
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("Create")]
    public async Task<IActionResult> CreateAsync(ApplicationUpload application)
    {
        if (application.RedirectAuthorize.Contains("code="))
            ModelState.AddModelError(nameof(ApplicationUpload.RedirectAuthorize), "Invalid URl!");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Account? account = ClientAccount;

        Application dbApplication = application.ToDataModel();
        dbApplication.OwnerId = account.Id;

        await db.Applications.AddAsync(dbApplication);
        await db.SaveChangesAsync();

        ApplicationResult result = new(dbApplication);

        return Ok(result);
    }

    [HttpGet]
    [Route("List")]
    [ProducesResponseType(typeof(ApplicationResult[]), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> ListAsync()
    {
        Account? account = ClientAccount;

        Application[] applications = await (from app in db.Applications
                                            where app.OwnerId == account.Id
                                            select app).ToArrayAsync();

        ApplicationResult[] applicationResults = applications.Select(sl => new ApplicationResult(sl)).ToArray();

        return Ok(applicationResults);
    }

    [HttpGet]
    [Route("Get")]
    public async Task<IActionResult> GetAsync(string client_key)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("Update")]
    [ProducesResponseType(typeof(ApplicationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAsync(int id)
    {
        throw new NotImplementedException();
    }
}

