namespace Nexus.OAuth.Dal.Models;

public class FirstStep
{
    /// <summary>
    /// First Step ID
    /// </summary>
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    /// <summary>
    /// Account FirstStep Id
    /// </summary>
    [Required]
    public int AccountId { get; set; }
    /// <summary>
    /// Date of Step
    /// </summary>
    [Required]
    public DateTime Date { get; set; }
    /// <summary>
    /// Step token
    /// </summary>
    [Required]
    [StringLength(512, MinimumLength = 32)]
    public string Token { get; set; }
    /// <summary>
    /// Is Valid Step
    /// </summary>
    [Required]
    public bool IsValid { get; set; }
    /// <summary>
    /// Step Client Ip Adress
    /// </summary>
    [Required]
    [MaxLength(16)]
    [Column("IpAdress")]
    public byte[] Ip { get; set; }
    /// <summary>
    /// Step Client unique Key
    /// </summary>
    [Required]
    [StringLength(256, MinimumLength = 16)]
    public string ClientKey { get; set; }
    /// <summary>
    /// Step Client User-Agent Header
    /// </summary>
    [Required]
    [StringLength(250, MinimumLength = 5)]
    public string UserAgent { get; set; }
    /// <summary>
    /// Step client redirect adress
    /// </summary>
    [StringLength(2048)]
    public string? Redirect { get; set; }


    [ForeignKey(nameof(AccountId))]
    public Account AccountNavigation { get; set; }
}

