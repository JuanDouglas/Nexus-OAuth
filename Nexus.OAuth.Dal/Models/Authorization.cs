namespace Nexus.OAuth.Dal.Models;

public class Authorization
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// Authorization Unique Code
    /// </summary>
    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string Code { get; set; }

    /// <summary>
    /// Date of Authorization
    /// </summary>
    [Required]
    public DateTime Date { get; set; }

    /// <summary>
    /// Max time age of this authorization
    /// </summary>
    public double? ExpiresIn { get; set; }

    /// <summary>
    /// Client state token
    /// </summary>
    [StringLength(256)]
    public string? State { get; set; }

    [Required]
    public bool IsValid { get; set; }

    [Required]
    public bool Used { get; set; }

    /// <summary>
    /// Saved Scopes Enum values
    /// </summary>
    [Required]
    [MinLength(1)]
    public byte[] ScopesBytes { get; set; }

    /// <summary>
    /// User 
    /// </summary>
    public string? ClientKey { get; set; }

    /// <summary>
    /// Application Authorization scopes
    /// </summary>
    [NotMapped]
    public Scope[] Scopes
    {
        get => ScopesBytes.Select(sl => (Scope)sl)
            .Distinct()
            .ToArray();

        set => ScopesBytes = value.Select(sl => (byte)sl)
            .Distinct()
            .ToArray();
    }

    [Required]
    public int ApplicationId { get; set; }
    [Required]
    public int AccountId { get; set; }

    [ForeignKey(nameof(AccountId))]
    public Account Account { get; set; }
    [ForeignKey(nameof(ApplicationId))]
    public Application Application { get; set; }
}
