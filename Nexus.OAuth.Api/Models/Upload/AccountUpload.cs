using Nexus.OAuth.Domain;
using Nexus.Tools.Validations.Attributes;

namespace Nexus.OAuth.Api.Models.Upload;
/// <summary>
/// Account Model
/// </summary>
public class AccountUpload : UploadModel<Account>
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


    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Account ToDataModel() => new()
    {
        Created = DateTime.UtcNow,
        Email = Email,
        Password = GeneralHelpers.HashPassword(Password),
        Name = Name,
        Phone = Phone,
#if DEBUG || LOCAL
        ConfirmationStatus = ConfirmationStatus.Complet
#else
        ConfirmationStatus = ConfirmationStatus.NotValided
#endif
    };

    public override void UpdateModel(ref Account model) => UpdateModel<AccountUpload>(ref model);
}