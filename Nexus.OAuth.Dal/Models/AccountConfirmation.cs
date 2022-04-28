namespace Nexus.OAuth.Dal.Models;

public class AccountConfirmation
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
    [StringLength(96, MinimumLength = 3)]
    public string Token { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    public bool Valid { get; set; }

    [Required]
    public ConfirmationType Type { get; set; }
    [Required]
    [ForeignKey(nameof(AccountId))]
    public Account AccountNavigation { get; set; }
}

