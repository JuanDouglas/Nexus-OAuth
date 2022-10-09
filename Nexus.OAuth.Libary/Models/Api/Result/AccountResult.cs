using Nexus.OAuth.Libary.Models.Api.Result.Enums;

namespace Nexus.OAuth.Libary.Models.Api.Result;

internal class AccountResult
{
    /// <summary>
    /// User Account Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// User name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// User Account E-mail
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// User account phone number.
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// Account creation date
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// User date of birth
    /// </summary>
    public DateTime DateOfBirth { get; set; }

    /// <summary>
    /// User Culture 
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    ///  Account Validation status
    /// </summary>
    public ValidationStatus ConfirmationStatus { get; set; }

    /// <summary>
    /// Account Profile Image
    /// </summary>
    public FileResult ProfileImage { get; set; }
}