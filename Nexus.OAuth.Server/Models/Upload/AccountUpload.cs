using Nexus.Tools.Validations.Attributes;

namespace Nexus.OAuth.Server.Models.Upload;
/// <summary>
/// Account Model
/// </summary>
public class AccountUpload
{
    /// <summary>
    /// User Account Name
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; }

    /// <summary>
    /// Account E-mail
    /// </summary>
    [Required]
    [EmailAddress]
    [StringLength(500, MinimumLength = 3)]
    [UniqueInDataBase(typeof(OAuthContext), typeof(Account), nameof(Account.Email))]
    public string Email { get; set; }

    /// <summary>
    /// Account Phone number.
    /// </summary>
    [Phone]
    [Required]
    public string Phone { get; set; }

    /// <summary>
    /// Account authentication password.
    /// </summary>
    [Required]
    [Password]
    [StringLength(30, MinimumLength = 8)]
    public string Password { get; set; }

    internal Account DbModel() => new()
    {
        Created = DateTime.UtcNow,
        Email = Email,
        Password = Password,
        Name = Name,
#if DEBUG
        ValidationStatus = ValidationStatus.Complet
#else
   ValidationStatus = ValidationStatus.NotValided
#endif
    };
}

