namespace Nexus.OAuth.Api.Models.Result;

public class FileResult
{
    public FileResult(Dal.Models.File file)
    {
        Id = file.Id;
        FileName = file.FileName;
        Type = file.Type;
        Length = file.Length;   
    }

    public int Id { get; set; }
    public string FileName { get; set; }
    public FileType Type { get; set; }
    public long Length { get; set; }
}

