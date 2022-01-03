using Nexus.OAuth.Api.Controllers.Base;
using System.Collections.Specialized;
using System.Web;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// Applications controller
/// </summary>
public class ApplicationsController : ApiController
{

    public const int ApplicationKeyLength = 32;
    public const int ApplicationSecretLength = 96;
    /// <summary>
    /// Private words
    /// </summary>
    public static readonly string[] privateWords = new string[] {
        "code",
        "error",
        "error_description"};


    /// <summary>
    /// Create a new application
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("Create")]
    public async Task<IActionResult> CreateAsync(ApplicationUpload application)
    {
        #region Valid Model
        if (!CheckPrivateWords(application.RedirectAuthorize))
        {
            ModelState.AddModelError(nameof(ApplicationUpload.RedirectAuthorize), $"The url query should not contain value for the private words: {privateWords}");
        }

        if (!CheckPrivateWords(application.RedirectLogin))
        {
            ModelState.AddModelError(nameof(ApplicationUpload.RedirectLogin), $"The url query should not contain value for the private words: {privateWords}");
        }

        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        #endregion

        Account? account = ClientAccount;

        Application dbApplication = application.ToDataModel();
        dbApplication.OwnerId = account.Id;

        await db.Applications.AddAsync(dbApplication);
        await db.SaveChangesAsync();

        ApplicationResult result = new(dbApplication);

        return Ok(result);
    }

    /// <summary>
    /// List Your applications with you are owner
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("MyApplications")]
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

    /// <summary>
    /// Gets a specific application by your client id.
    /// </summary>
    /// <param name="client_id">Application key (client_id).</param>
    /// <returns></returns>
    [HttpGet]
    [Route("ByClientId")]
    [AllowAnonymous]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(Application), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAsync(string client_id)
    {
        if (string.IsNullOrEmpty(client_id))
            return BadRequest();

        Application application = await (from app in db.Applications
                                         where app.Key == client_id
                                         select app).FirstOrDefaultAsync();
        if (application == null)
            return NotFound();

        ApplicationResult result = new(application);
        result.Secret = string.Empty;
        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("Update")]
    [RequireAuthentication]
    [ProducesResponseType(typeof(ApplicationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAsync(int id)
    {
        throw new NotImplementedException();
    }

    private static bool CheckPrivateWords(string url)
    {
        bool valid = true;
        Uri uri = new(url);
        NameValueCollection query = HttpUtility.ParseQueryString(uri.Query);

        foreach (string word in privateWords)
        {
            valid = string.IsNullOrEmpty(query[word]);

            if (!valid)
                break;
        }

        return valid;
    }
}

