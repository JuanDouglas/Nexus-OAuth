using Nexus.OAuth.Api.Controllers;
using Nexus.Tools.Validations.Attributes;

namespace Nexus.OAuth.Api.Models.Upload;

/// <summary>
/// Application Upload Model
/// </summary>
public class ApplicationUpload : UploadModel<Application>
{

    /// <summary>
    /// Application Name
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 5)]
    public string Name { get; set; }

    /// <summary>
    /// Application Description
    /// </summary>
    [Required(AllowEmptyStrings = false)]
    [StringLength(2500, MinimumLength = 3)]
    public string Description { get; set; }

    /// <summary>
    /// Appplication Redirect Login URL
    /// </summary>
    [Url]
    [Required]
    [StringLength(1024, MinimumLength = 6)]
    public string RedirectLogin { get; set; }

    /// <summary>
    /// Application work status
    /// </summary>
    [Required]
    public ApplicationStatus Status { get; set; }

    /// <summary>
    /// Application Authorize Login redirect URL
    /// </summary>
    [Url]
    [Required]
    [StringLength(1024, MinimumLength = 6)]
    public string RedirectAuthorize { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Application ToDataModel() => new()
    {
        Name = Name,
        RedirectLogin = RedirectLogin,
        RedirectAuthorize = RedirectAuthorize,
        Status = Status,
        LogoId = null,
        Description = Description,
        Secret = GeneralHelpers.GenerateToken(ApplicationsController.ApplicationSecretLength),
        Key = GeneralHelpers.GenerateToken(ApplicationsController.ApplicationKeyLength, upper: false),
    };

    public override void UpdateModel(ref Application model) => UpdateModel<ApplicationUpload>(ref model);
}

