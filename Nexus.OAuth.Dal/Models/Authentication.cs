
namespace Nexus.OAuth.Dal.Models;
public class Authentication
{
    /// <summary>
    /// Authentication ID
    /// </summary>
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Refresh authentication token
    /// </summary>
    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string RefreshToken { get; set; }

    /// <summary>
    /// Authentication Token Type
    /// </summary>
    [Required]
    public TokenType TokenType { get; set; }

    /// <summary>
    /// Authentication Date
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Authentication unique token.
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 96)]
    public string Token { get; set; }

    /// <summary>
    /// Max token using time 
    /// </summary>
    public double? ExpiresIn { get; set; }

    /// <summary>
    /// Authentication First Step Id (Client login only)
    /// </summary>
    public int? FirstStepId { get; set; }

    /// <summary>
    /// Application Authorization Id (Applications only)
    /// </summary>
    public int? AuthorizationId { get; set; }

    /// <summary>
    /// QrCode Authorization reference (Qr Code login Only)
    /// </summary>
    public int? QrCodeAuthorizationId { get; set; }

    /// <summary>
    /// IP Adress for Client Authentication.
    /// </summary>
    [Required]
    [MaxLength(16)]
    [Column("IpAdress")]
    public byte[] Ip { get; set; }

    /// <summary>
    /// Indicates whether this authentication token is still valid.
    /// </summary>
    [Required]
    public bool IsValid { get; set; }

    [ForeignKey(nameof(FirstStepId))]
    public virtual FirstStep? FirstStepNavigation { get; set; }
    [ForeignKey(nameof(AuthorizationId))]
    public virtual Authorization? AuthorizationNavigation { get; set; }
    [ForeignKey(nameof(QrCodeAuthorizationId))]
    public virtual QrCodeAuthorization? QrCodeAuthorizationNavigation { get; set; }
}

