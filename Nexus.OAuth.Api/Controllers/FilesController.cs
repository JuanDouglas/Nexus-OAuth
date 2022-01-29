using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Nexus.OAuth.Api.Controllers.Base;
using Nexus.OAuth.Domain.Storage;
using File = Nexus.OAuth.Dal.Models.File;
using FileAccess = Nexus.OAuth.Dal.Models.Enums.FileAccess;

namespace Nexus.OAuth.Api.Controllers;

public class FilesController : ApiController
{
    public FilesController(IConfiguration configuration) : base(configuration)
    {
    }

    [HttpGet]
    [Route("Images/Download")]
    public async Task<IActionResult> GetImageAsync(string fileName, ResourceType resourceType)
                                                                                                                                                                                                           {
        Account account = ClientAccount;

        File file = await (from fl in db.Files
                           where fl.FileName == fileName &&
                                 fl.Type == FileType.Image &&
                                 (fl.Access == FileAccess.Public || fl.ResourceOwnerId == account.Id)
                           select fl).FirstOrDefaultAsync();

        if (file == null)
            return NotFound();

        try
        {
            byte[] fileBytes = await FileStorage.ReadFileAsync(file.Type, DirectoryByResourceType(resourceType), file.FileName);




            return File(fileBytes, "image/png");
        }
        catch (FileNotFoundException ex)
        {
            return BadRequest();
        }
    }

    [NonAction]
    private static DirectoryType DirectoryByResourceType(ResourceType resourceType) => resourceType switch
    {
        ResourceType.ApplicationLogo => DirectoryType.ApplicationsLogo,
        _ => DirectoryType.ApplicationsLogo
    };
}

