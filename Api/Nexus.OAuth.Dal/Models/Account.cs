using System.ComponentModel;

namespace Nexus.OAuth.Dal.Models;

/// <summary>
/// User Account Database Model
/// </summary>
public class Account
{
    /// <summary>
    /// User Account Id
    /// </summary>
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    /// <summary>
    /// User name.
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Name { get; set; }
    /// <summary>
    /// User Account E-mail
    /// </summary>
    [Required]
    [StringLength(500, MinimumLength = 3)]
    public string Email { get; set; }
    /// <summary>
    /// User account phone number.
    /// </summary>
    [Required]
    [StringLength(30, MinimumLength = 9)]
    public string Phone { get; set; }
    /// <summary>
    /// User Account Login Password 
    /// </summary>
    [Required]
    [StringLength(96, MinimumLength = 8)]
    public string Password { get; set; }
    /// <summary>
    /// Account creation date
    /// </summary>
    [Required]
    public DateTime Created { get; set; }
    /// <summary>
    /// User date of birth
    /// </summary>
    [Required]
    public DateTime DateOfBirth { get; set; }
    /// <summary>
    /// User Culture 
    /// </summary>
    [Required]
    [DefaultValue("pt-BR")]
    [StringLength(10, MinimumLength = 5)]
    public string Culture { get; set; }
    /// <summary>
    ///  Account Validation status
    /// </summary>
    [Required]
    public ConfirmationStatus ConfirmationStatus { get; set; }
    /// <summary>
    /// 
    /// </summary>
    public int? ProfileImageID { get; set; }

    [ForeignKey(nameof(ProfileImageID))]
    public File? ProfileImage { get; set; }
}

