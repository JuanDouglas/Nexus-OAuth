namespace Nexus.OAuth.Dal.Models;

public class Application
{
    public int Id { get; set; }

    /// <summary>
    /// Application Name
    /// </summary>
    [Required]
    [StringLength(150, MinimumLength = 5)]
    public string Name { get; set; }

    [Required]
    public int OwnerId { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 96)]
    public string Secret { get; set; }

    [Required]
    [StringLength(500, MinimumLength = 16)]
    public string Key { get; set; }

    [ForeignKey(nameof(OwnerId))]
    public Account Owner { get; set; }

}

