namespace Nexus.OAuth.Api.Models.Result;

public class FileResult
{
    public FileResult(Dal.Models.File file)
    {
    }

    public int Id { get; set; }
    public string FileName { get; set; }
    public FileType Type { get; set; }
    public long Length { get; set; }
}

