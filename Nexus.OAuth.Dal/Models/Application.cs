using System.ComponentModel;

namespace Nexus.OAuth.Dal.Models;

public class Application
{
    public int Id { get; set; }

    /// <summary>
    /// Application Name
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 5)]
    public string Name { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string Secret { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 16)]
    public string Key { get; set; }

    [Required]
    [StringLength(2500, MinimumLength = 5)]
    public string Description { get; set; }

    [Required]
    [DefaultValue("example.com")]
    [StringLength(500, MinimumLength = 6)]
    public string Site { get; set; }

    [Required]
    public ApplicationStatus Status { get; set; }

    [Required]
    [StringLength(1024, MinimumLength = 6)]
    public string RedirectLogin { get; set; }

    [Required]
    [StringLength(1024, MinimumLength = 6)]
    public string RedirectAuthorize { get; set; }
    public ConfirmationStatus? MinConfirmationStatus { get; set; }

    public int? LogoId { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public Account Owner { get; set; }

    [ForeignKey(nameof(LogoId))]
    public File? Logo { get; set; }

}

