using Nexus.OAuth.Libary.Models.Enums;

namespace Nexus.OAuth.Libary.Models.Api.Result;
internal class FileResult
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public long Length { get; set; }
    public FileType Type { get; set; }
    public ResourceType ResourceType { get; set; }
}
