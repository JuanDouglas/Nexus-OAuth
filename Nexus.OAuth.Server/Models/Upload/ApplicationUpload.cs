using Nexus.OAuth.Server.Controllers;
using Nexus.OAuth.Server.Controllers.Base;
using Nexus.Tools.Validations.Attributes;

namespace Nexus.OAuth.Server.Models.Upload;

public class ApplicationUpload : IUploadModel<Application>
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
    /// Application Authorize Login redirect URL
    /// </summary>
    [Required]
    [StringLength(1024, MinimumLength = 6)]
    public string RedirectAuthorize { get; set; }


    public Application ToDataModel() => new()
    {
        Name = Name,
        RedirectLogin = RedirectLogin,
        RedirectAuthorize = RedirectAuthorize,
        Secret = ApiController.GenerateToken(ApplicationsController.ApplicationSecretLength),
        Key = ApiController.GenerateToken(ApplicationsController.ApplicationKeyLength, upper: false),
    };
}

