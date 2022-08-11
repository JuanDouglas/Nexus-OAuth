using FileAccess = Nexus.OAuth.Dal.Models.Enums.FileAccess;

namespace Nexus.OAuth.Dal.Models;

public class File
{
    public const int DefaultImageId = 1;

    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(1200, MinimumLength = 3)]
    public string FileName { get; set; }

    [Required]
    public FileType Type { get; set; }

    [Required]
    public DirectoryType DirectoryType { get; set; }

    [Required]
    public DateTime Inserted { get; set; }

    public int? ResourceOwnerId { get; set; }

    [Required]
    public FileAccess Access { get; set; }

    [Required]
    public long Length { get; set; }

    [ForeignKey(nameof(ResourceOwnerId))]
    public Account ResourceOwner { get; set; }
}

