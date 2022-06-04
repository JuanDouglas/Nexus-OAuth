using Nexus.OAuth.Web.Models.Enums;
using Nexus.Tools.Validations.Attributes;

namespace Nexus.OAuth.Web.Models;

public class Application
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
    /// Application owner site
    /// </summary>
    [Url]
    [Required]
    [StringLength(250, MinimumLength = 6)]
    public string Site { get; set; }

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
    /// Minimuns confirmation type authorize
    /// </summary>
    public ConfirmationStatus? MinConfirmationStatus { get; set; }

}
