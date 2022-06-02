using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Api.Properties;
using Nexus.OAuth.Domain.Storage;
using SixLabors.ImageSharp;
using File = Nexus.OAuth.Dal.Models.File;
using FileAccess = Nexus.OAuth.Dal.Models.Enums.FileAccess;

namespace Nexus.OAuth.Api.Controllers;

[AllowAnonymous]
public class FilesController : ApiController
{
    public FilesController(IConfiguration configuration) : base(configuration)
    {
    }

    /// <summary>
    /// Download as image
    /// </summary>
    /// <param name="fileName">Image filename</param>
    /// <param name="resourceType">Image Resource type</param>
    /// <param name="extension">Result extension</param>
    /// <param name="type">Type of file</param>
    /// <returns></returns>
    [HttpGet]
    [AllowAnonymous]
    [Route("{type}/Download")]
    public async Task<IActionResult> DownloadAsync([FromRoute] FileType type, string fileName, ResourceType resourceType, ImageExtension extension = ImageExtension.Png)
    {
        byte[] fileBytes;
        if (type == FileType.Template && resourceType == ResourceType.DefaultFile)
            fileBytes = FileByResourceType(fileName);
        else
        {
            try
            {
                fileBytes = await GetFileAsync(fileName, resourceType);
            }
            catch (FileNotFoundException)
            {
                return NotFound();
            }
        }

        if (extension != ImageExtension.Png)
        {
            MemoryStream ms = new(fileBytes);
            Image image;

            using (ms)
            {
                image = await Image.LoadAsync(ms);
            }

            ms = new();

            fileBytes = await SaveImageAsync(image, extension, ms);
            return File(fileBytes, $"image/{Enum.GetName(extension)?.ToLowerInvariant() ?? "png"}");
        }

        return File(fileBytes, "image/png");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    /// <exception cref="FileNotFoundException"></exception>
    [NonAction]
    public async Task<byte[]> GetFileAsync(string fileName, ResourceType resourceType)
    {
        Account? account = ClientAccount;
        int? accountId = account?.Id;

        File file = await (from fl in db.Files
                           where fl.FileName == fileName &&
                                 fl.Type == FileType.Image &&
                                 (fl.Access == FileAccess.Public || fl.ResourceOwnerId == accountId)
                           select fl).FirstOrDefaultAsync();

        if (file == null)
            throw new FileNotFoundException();

        return await FileStorage.ReadFileAsync(file.Type, DirectoryByResourceType(resourceType), file.FileName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="resourceType"></param>
    /// <returns></returns>
    [NonAction]
    public static DirectoryType DirectoryByResourceType(ResourceType resourceType)
        => (DirectoryType)(resourceType);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    [NonAction]
    public byte[] FileByResourceType(string fileName) => fileName switch
    {
        ApplicationResult.DefaultLogo => Resources.application,
        AccountResult.DefaultProfile => Resources.account,
        _ => Array.Empty<byte>()
    };
}