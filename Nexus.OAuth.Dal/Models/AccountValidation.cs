namespace Nexus.OAuth.Dal.Models;

public class AccountValidation
{
    /// <summary>
    /// User Account Validation Id
    /// </summary>
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// 
    /// </summary>
    [Required]
    public int AccountId { get; set; }


    [Required]
    [ForeignKey(nameof(AccountId))]
    public Account AccountNavigation { get; set; }
}

