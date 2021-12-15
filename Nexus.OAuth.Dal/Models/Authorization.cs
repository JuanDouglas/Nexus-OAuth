namespace Nexus.OAuth.Dal.Models;

public class Authorization
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Authorization Unique Token
    /// </summary>
    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string Token { get; set; }
    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string RefreshToken { get; set; }
    [Required]
    public DateTime Created { get; set; }
    public TimeSpan? ExpiresIn { get; set; }
    [Required]
    public TokenType TokenType { get; set; }
    [Required]
    public int ApplicationId { get; set; }
    [Required]
    public int AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; }
    [ForeignKey(nameof(ApplicationId))]
    public Application Application { get; set; }
}
