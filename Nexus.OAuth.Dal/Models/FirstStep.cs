
namespace Nexus.OAuth.Dal.Models;
public class FirstStep
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public int AccountId { get; set; }
    [Required]
    public DateTime Date { get; set; }
    [Required]
    public string Token { get; set; }
    [Required]
    public bool IsValid { get; set; }
    [Required]
    public string IpAdress { get; set; }
    [Required]
    public string Key { get; set; }
    [Required]
    public string Redirect { get; set; }


    [ForeignKey(nameof(AccountId))]
    public Account AccountNavigation { get; set; }
}

