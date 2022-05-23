namespace Nexus.OAuth.Dal.Models;

public class QrCodeReference
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(150, MinimumLength = 6)]
    public string Code { get; set; }

    [Required]
    public DateTime Create { get; set; }

    public DateTime? Use { get; set; }

    [Required]
    [StringLength(256, MinimumLength = 32)]
    public string ClientKey { get; set; }

    [Required]
    public bool Valid { get; set; }

    [Required]
    public bool Used { get; set; }

    /// <summary>
    /// Step Client User-Agent Header
    /// </summary>
    [Required]
    [StringLength(250, MinimumLength = 32)]
    public string ValidationToken { get; set; }

    /// <summary>
    /// Step Client User-Agent Header
    /// </summary>
    [Required]
    [StringLength(250, MinimumLength = 5)]
    public string UserAgent { get; set; }
    /// <summary>
    /// Step Client Ip Adress
    /// </summary>
    [Required]
    [MaxLength(6)]
    [Column("IpAdress")]
    public byte[] Ip { get; set; }
}

