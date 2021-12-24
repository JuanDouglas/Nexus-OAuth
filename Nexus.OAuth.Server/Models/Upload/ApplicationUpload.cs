using Nexus.OAuth.Api.Controllers;
using Nexus.OAuth.Api.Controllers.Base;
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
    /// Appplication Redirect Login URL
    /// </summary>
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
    [Required]
    [StringLength(1024, MinimumLength = 6)]
    public string RedirectAuthorize { get; set; }


    public override Application ToDataModel() => new()
    {
        Name = Name,
        RedirectLogin = RedirectLogin,
        RedirectAuthorize = RedirectAuthorize,
        Status = Status,
        Secret = ApiController.GenerateToken(ApplicationsController.ApplicationSecretLength),
        Key = ApiController.GenerateToken(ApplicationsController.ApplicationKeyLength, upper: false),
    };

    public override void UpdateModel(ref Application model) => UpdateModel<ApplicationUpload>(ref model);
}

