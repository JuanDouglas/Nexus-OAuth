using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Api.Properties;
using Nexus.OAuth.Domain.Storage;
using Nexus.OAuth.Domain.Storage.Enums;
using SixLabors.ImageSharp;
using System.Web;
using File = Nexus.OAuth.Dal.Models.File;
using FileAccess = Nexus.OAuth.Dal.Models.Enums.FileAccess;
using FileResult = Nexus.OAuth.Api.Models.Result.FileResult;

namespace Nexus.OAuth.Api.Controllers;
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Derefence of a possible null reference.

/// <summary>
/// Accounts Controller
/// </summary>
[RequireAuthentication(RequireAccountValidation = false, RequiresToBeOwner = false, MinAuthenticationLevel = (int)Scope.User)]
public class AccountController : ApiController
{
    private const string redirectSufix = "--redirect--";
    private const string nameSufix = "--name--";
    private const string maxTimeSufix = "--max--";
    private const double minConfirmationPeriod = 900000;
    private const int maxConfirmationsForPeriod = 5;
    private const double maxConfirmationPeriod = minConfirmationPeriod * 2;
    public AccountController(IConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Register a new account.
    /// </summary>
    /// <param name="account">Register Account Model</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPut, Route("Register")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> RegisterAsync([FromBody] AccountUpload account)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        Account dbAccount = account.ToDataModel();

        await db.Accounts.AddAsync(dbAccount);
        await db.SaveChangesAsync();

        AccountResult result = new(dbAccount);

        return Ok(result);
    }

    /// <summary>
    /// Get Client Account informations
    /// </summary>
    /// <returns></returns>
    [HttpGet, Route("MyAccount")]
    [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(AccountResult), (int)HttpStatusCode.OK)]
    public async Task<IActionResult> MyAccountAsync()
    {
        Account? account = ClientAccount;
        AccountResult result;

        if (account.ProfileImageID != null)
        {
            File profile = await (from fl in db.Files
                                  where fl.Id == account.ProfileImageID
                                  select fl).FirstOrDefaultAsync();

            result = new(account, profile);
        }
        else
            result = new(account);

        return Ok(result);
    }

    /// <summary>
    /// Send specific confirmation.
    /// </summary>
    /// <param name="type">Type of confirmation.</param>
    /// <returns></returns>
    [HttpPost, Route("SendConfirmation")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [ProducesResponseType((int)HttpStatusCode.TooManyRequests)]
    [RequireAuthentication(RequireAccountValidation = false, RequiresToBeOwner = true)]
    public async Task<IActionResult> SendConfirmationAsync(ConfirmationType type)
    {
        Account? account = ClientAccount;

        if ((sbyte)type < (sbyte)(account?.ConfirmationStatus ?? ConfirmationStatus.NotValided))
            return Conflict();

        DateTime minDate = DateTime.UtcNow - TimeSpan.FromMilliseconds(minConfirmationPeriod);
        int confirmations = await (from accConfirm in db.AccountConfirmations
                                   where accConfirm.AccountId == account.Id &&
                                         accConfirm.Date > minDate &&
                                         accConfirm.Type == type
                                   select accConfirm).CountAsync();

        if (confirmations >= maxConfirmationsForPeriod)
            return StatusCode((int)HttpStatusCode.TooManyRequests);

        AccountConfirmation confirmation = new()
        {
            AccountId = account.Id,
            Type = type,
            Date = DateTime.UtcNow,
            Valid = true
        };

        try
        {
            switch (type)
            {
                case ConfirmationType.PhoneNumber:
                    confirmation.Token = GeneralHelpers.GenerateToken(6, false, false);
                    break;
                case ConfirmationType.EmailAdress:
                    confirmation.Token = GeneralHelpers.GenerateToken(96);
                    string path = Configuration.GetValue<string>(WebHostDefaults.ContentRootKey);
                    string htmlContent = Resources.confirm_account;

                    var query = HttpUtility.ParseQueryString(string.Empty);
                    query["token"] = confirmation.Token;
                    query["type"] = Enum.GetName(type);

                    string redirect = $"{WebHost}Account/Confirm?{query}";

                    htmlContent = htmlContent.Replace(redirectSufix, redirect)
                        .Replace(nameSufix, account.Name.Split(' ').First() ?? "Unknown")
                        .Replace(maxTimeSufix, $"{TimeSpan.FromMilliseconds(minConfirmationPeriod).Minutes} min");

                    await EmailSender.SendEmailAsync(htmlContent, "Account verification", account.Email);
                    break;
            }
        }
        catch (Exception)
        {
            return StatusCode((int)HttpStatusCode.ServiceUnavailable);
        }

        db.AccountConfirmations.Add(confirmation);
        await db.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Confirm account using method (validationType method).
    /// </summary>
    /// <param name="type">Type of method for validation.</param>
    /// <param name="token">Validation code.</param>
    /// <returns></returns>
    [HttpPost, Route("Confirm")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    [RequireAuthentication(RequireAccountValidation = false, RequiresToBeOwner = true)]
    public async Task<IActionResult> ValidAccountAsync(ConfirmationType type, string token)
    {
        Account? account = ClientAccount;

        if ((sbyte)type < (sbyte)(account?.ConfirmationStatus ?? ConfirmationStatus.NotValided))
            return Conflict();

        AccountConfirmation? confirmation = await (from accConfirm in db.AccountConfirmations
                                                   where accConfirm.Valid &&
                                                         accConfirm.AccountId == account.Id &&
                                                         accConfirm.Type == type &&
                                                         accConfirm.Token == token
                                                   select accConfirm).FirstOrDefaultAsync();
        if (confirmation == null)
            return NotFound();

        DateTime maxDate = DateTime.UtcNow + TimeSpan.FromMilliseconds(maxConfirmationPeriod);

        if (confirmation.Date > maxDate)
        {
            confirmation.Valid = false;
            await db.SaveChangesAsync();
            return NotFound();
        }

        account = await (from acc in db.Accounts
                         where acc.Id == account.Id
                         select acc).FirstOrDefaultAsync();

        account.ConfirmationStatus = (ConfirmationStatus)(type + 1);
        confirmation.Valid = false;

        await db.SaveChangesAsync();

        return Ok();
    }

    /// <summary>
    /// Set profile image
    /// </summary>
    /// <param name="formFile">File of new profile image</param>
    /// <returns>Ok result for model <see cref="FileResult"/></returns>
    [HttpPut, Route("ProfileImage")]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(FileResult), (int)HttpStatusCode.OK)]
    [RequireAuthentication(RequiresToBeOwner = true)]
    public async Task<IActionResult> SetProfileImageAsync([FromForm(Name = "ProfileImage")] IFormFile formFile)
    {
        Account? account = ClientAccount;

        if (formFile.Length > MaxImageSize)
            return BadRequest();

        account = await (from acc in db.Accounts
                         where acc.Id == account.Id
                         select acc).FirstOrDefaultAsync();

        try
        {
            Image convert = await Image.LoadAsync(formFile.OpenReadStream());

            #region Remove Previous Image
            if (account.ProfileImageID != null)
            {
                File previous = await (from prev in db.Files
                                       where prev.Id == account.ProfileImageID &&
                                             prev.Type == FileType.Image &&
                                             (prev.Access == FileAccess.Public || prev.ResourceOwnerId == account.Id)
                                       select prev).FirstOrDefaultAsync();
                if (previous != null)
                {
                    if (previous.DirectoryType != DirectoryType.Defaults &&
                        previous.Type != FileType.Template)
                    {
                        account.ProfileImageID = null;

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

            (string fileName, string directory) = await FileStorage.WriteFileAsync(FileType.Image, DirectoryType.AccountsProfile, Extension.png, bytes);

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

            account.ProfileImageID = file.Id;

            await db.SaveChangesAsync();
            #endregion

            FileResult result = new(file, ResourceType.AccountProfile);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest();
        }
    }

    [HttpPost, Route("Update")]
    [ProducesResponseType((int)HttpStatusCode.NotImplemented)]
    public async Task<IActionResult> UpdateAsync([FromBody] Account model, [FromQuery] int id)
        => StatusCode((int)HttpStatusCode.NotImplemented);

}