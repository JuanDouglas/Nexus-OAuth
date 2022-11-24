using Nexus.OAuth.Libary.Controllers.Base;
using Nexus.OAuth.Libary.Models;
using Nexus.OAuth.Libary.Models.Enums;
using System.Net;
using System.Web;

namespace Nexus.OAuth.Libary.Controllers;

internal class FilesController : AuthorizedController
{
    public FilesController(string clientKey, string authorization, TokenType tokenType)
        : base(clientKey, authorization, tokenType)
    {
    }

    public async Task<byte[]> DownloadAsync(FileType type, string fileName, ResourceType rsType, Extension extension)
    {
        string url = $"{apiHost}Files/{Enum.GetName(type)}/Download?" +
            $"fileName={HttpUtility.UrlEncode(fileName)}" +
            $"&resourceType={Enum.GetName(rsType)}" +
            $"&extension={Enum.GetName(extension)}";

        HttpRequestMessage request = defaultRequest;
        request.RequestUri = new(url);

        var response = await httpClient.SendAsync(request);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnauthorizedException();
        }

        return await response.Content.ReadAsByteArrayAsync();
    }
}