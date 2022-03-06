using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Domain.Storage;
using Nexus.OAuth.Domain.Storage.Enums;
using SixLabors.ImageSharp;
using System.Collections.Specialized;
using System.Web;
using File = Nexus.OAuth.Dal.Models.File;
using FileAccess = Nexus.OAuth.Dal.Models.Enums.FileAccess;
using FileResult = Nexus.OAuth.Api.Models.Result.FileResult;

namespace Nexus.OAuth.Api.Controllers;

/// <summary>
/// Applications controller
/// </summary>
[RequireAuthentication(RequireAccountValidation = true)]
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

    public ApplicationsController(IConfiguration configuration) : base(configuration)
    {
    }


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

        File logo = (from img in db.Files
                     where img.Type == FileType.Image &&
                           img.Id == dbApplication.LogoId
                     select img).FirstOrDefault();

        ApplicationResult result = new(dbApplication, logo);

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

        int[] imgsId = applications
            .Select(sl => sl.LogoId ?? Dal.Models.File.DefaultImageId)
            .Distinct()
            .ToArray();

        File[] images = await (from imgs in db.Files
                               where imgsId.Contains(imgs.Id)
                               select imgs).ToArrayAsync();

        List<ApplicationResult> results = new();
        foreach (var item in applications)
        {
            File? image = (from fs in images
                           where fs.Id == (item.LogoId ?? -1)
                           select fs).FirstOrDefault();

            results.Add(new(item, image));
        }
        return Ok(results.ToArray());
    }

    /// <summary>
    /// Gets a specific application by your client id.
    /// </summary>
    /// <param name="client_id">Application key (client_id).</param>
    /// <param name="secrets">Indicates whether to return the application's client secret (only works with the application's owner)</param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("ByClientId")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ApplicationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> GetAsync(string client_id, bool secrets = false)
    {
        Account? account = ClientAccount;

        if (string.IsNullOrEmpty(client_id))
            return BadRequest();

        Application application = await (from app in db.Applications
                                         where app.Key == client_id
                                         select app).FirstOrDefaultAsync();
        if (application == null)
            return NotFound();

        File logo = (from img in db.Files
                     where img.Type == FileType.Image &&
                           img.Id == application.LogoId
                     select img).FirstOrDefault();

        ApplicationResult result = new(application, logo);

        if ((application.OwnerId != (account?.Id ?? -1)) && !secrets)
        {
            application.Secret = string.Empty;
        }

        return Ok(result);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">Application Id</param>
    /// <param name="updateSecret"></param>
    /// <param name="upload">Update model</param>
    /// <returns></returns>
    [HttpPost]
    [Route("Update")]
    [RequireAuthentication]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ApplicationResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> UpdateAsync([FromQuery] int id, [FromBody] ApplicationUpload upload, [FromQuery] bool updateSecret = false)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Account account = ClientAccount ?? new() { Id = -1 };

        Application? application = await (from app in db.Applications
                                          where app.Id == id &&
                                                app.OwnerId == account.Id
                                          select app).FirstOrDefaultAsync();

        if (application == null)
            return NotFound();

        upload.UpdateModel(application);

        if (string.IsNullOrEmpty(application.Secret) || updateSecret)
        {
            application.Secret = GeneralHelpers.GenerateToken(ApplicationSecretLength);
        }

        db.Entry(application).State = EntityState.Modified;

        await db.SaveChangesAsync();

        File logo = (from img in db.Files
                     where img.Type == FileType.Image &&
                           img.Id == application.LogoId
                     select img).FirstOrDefault();

        ApplicationResult result = new(application, logo);

        return Ok(result);
    }

    /// <summary>
    /// Defines the application logo
    /// </summary>
    /// <param name="formFile"></param>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    [HttpPut]
    [Route("SetLogo")]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> AddLogoImageAsync([FromForm(Name = "Logo")] IFormFile formFile, [FromQuery] int applicationId)
    {
        Account? account = ClientAccount;

        if (formFile == null)
            return BadRequest();

        if (formFile.Length > MaxImageSize)
            return BadRequest();

        try
        {
            Application? application = await (from app in db.Applications
                                              where app.Id == applicationId &&
                                                    app.OwnerId == account.Id
                                              select app).FirstOrDefaultAsync();

            if (application == null)
                return NotFound();

            Image convert = await Image.LoadAsync(formFile.OpenReadStream());


            #region Remove Previous Image
            if (application.LogoId != null)
            {
                File previous = await (from prev in db.Files
                                       where prev.Id == application.LogoId &&
                                             prev.Type == FileType.Image &&
                                             (prev.Access == FileAccess.Public || prev.ResourceOwnerId == account.Id)
                                       select prev).FirstOrDefaultAsync();
                if (previous != null)
                {
                    if (previous.DirectoryType != DirectoryType.Defaults &&
                        previous.Type != FileType.Template)
                    {
                        application.LogoId = null;

                        await db.SaveChangesAsync();

                        db.Entry(previous).State = EntityState.Deleted;

                        await db.SaveChangesAsync();

                        await FileStorage.DeleteFileAsync(previous.Type, previous.DirectoryType, previous.FileName);
                    }
                }
            }
            #endregion

            #region Load and Save Image
            using MemoryStream ms = new();

            await convert.SaveAsPngAsync(ms);

            byte[] bytes = await Task.Run(() => ms.ToArray());

            (string fileName, string directory) = await FileStorage.WriteFileAsync(FileType.Image, DirectoryType.ApplicationsLogo, Extension.png, bytes);

            File file = new()
            {
                Length = bytes.Length,
                DirectoryType = DirectoryType.ApplicationsLogo,
                Type = FileType.Image,
                FileName = fileName,
                ResourceOwnerId = account.Id,
                Access = FileAccess.Public,
                Inserted = DateTime.UtcNow
            };

            db.Files.Add(file);

            await db.SaveChangesAsync();

            application.LogoId = file.Id;

            await db.SaveChangesAsync();
            #endregion

            FileResult result = new(file, ResourceType.ApplicationLogo);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
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

