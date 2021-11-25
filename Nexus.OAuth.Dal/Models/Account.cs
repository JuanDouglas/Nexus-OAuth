using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nexus.OAuth.Dal.Models;

/// <summary>
/// User Account Database Model
/// </summary>
[Table("Account")]
public class Account
{
    /// <summary>
    /// User Account Id
    /// </summary>
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    /// <summary>
    /// User name.
    /// </summary>
    [Required]
    public string Name { get; set; }
    /// <summary>
    /// User Account E-mail
    /// </summary>
    [Required]
    [StringLength(500, MinimumLength = 3)]
    public string Email { get; set; }
    /// <summary>
    /// User Account Login Password 
    /// </summary>
    [Required]
    [StringLength(50, MinimumLength = 8)]
    public string Password { get; set; }
}

