using Nexus.OAuth.Api.Controllers.Base;
using System.Collections.Specialized;
using System.Web;

namespace Nexus.OAuth.Api.Controllers;

public class ApplicationsController : ApiController
{
    public const int ApplicationKeyLength = 32;
    public const int ApplicationSecretLength = 96;
    public static string[] privateWords = new string[] {
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

