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
    /// User date of birth
    /// </summary>
    [Required]
    public DateTime? DateOfBirth { get; set; }

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

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Compare(nameof(Password))]
    public string ConfirmPassword { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    [Boolean(TrueOnly = true)]
    public bool AcceptTerms { get; set; }

    /// <summary>
    /// User current culture
    /// </summary>
    [Required]
    [Culture]
    [StringLength(10, MinimumLength = 5)]
    public string Culture { get; set; }

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
        Culture = Culture,
        DateOfBirth = DateOfBirth ?? DateTime.MinValue,
#warning Check Default Confirmation Status 
#if DEBUG || LOCAL
        ConfirmationStatus = ConfirmationStatus.Support
#else
        ConfirmationStatus = ConfirmationStatus.NotValided
#endif
    };

    internal override void UpdateModel(in Account model) =>
        UpdateModel<AccountUpload>(model);
}