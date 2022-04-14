using Microsoft.AspNetCore.Cors;
using Nexus.OAuth.Domain.Authentication;
using Nexus.OAuth.Domain.Authentication.Exceptions;
using SixLabors.ImageSharp;

namespace Nexus.OAuth.Api.Controllers.Base;

/// <summary>
/// Base Application Controller
/// </summary>
[EnableCors]
[RequireHttps]
[ApiController]
[Route("api/[controller]")]
[RequireAuthentication(RequireAccountValidation = true, RequiresToBeOwner = false)]
public class ApiController : ControllerBase
{

    /// <summary>
    /// 
    /// </summary>
    public const string AuthorizationHeader = AuthenticationHelper.AuthorizationHeader;

    /// <summary>
    /// 
    /// </summary>
    public const string ClientKeyHeader = AuthenticationHelper.ClientKeyHeader;

    /// <summary>
    /// 
    /// </summary>
    public const string UserAgentHeader = "User-Agent";

    /// <summary>
    /// Max image load bytes size
    /// </summary>
    public const long MaxImageSize = 15000000; // Equal 15mb

    /// <summary>
    /// OAuth database context
    /// </summary>
    protected internal readonly OAuthContext db;

    protected internal readonly MongoDataContext mongoDb;
    /// <summary>
    /// Request Client Account 
    /// </summary>
    protected internal Account? ClientAccount
    {
        get
        {
            try
            {
                (TokenType tokenType, string[] tokens, _) = AuthenticationHelper.GetAuthorization(HttpContext);

                Task<Account?> accountTask = AuthenticationHelper.GetAccountAsync(tokenType, tokens[0]);
                accountTask.Wait();

                return accountTask.Result;
            }
            catch (AuthenticationException ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
    /// <summary>
    /// Client User-Agent
    /// </summary>
    public string UserAgent { get => Request.Headers.UserAgent.ToString(); }

    /// <summary>
    /// Client Remote Ip Adress
    /// </summary>
    public IPAddress? RemoteIpAdress { get => HttpContext.Connection.RemoteIpAddress; }

    public IConfiguration Configuration { get; private set; }
    public ApiController(IConfiguration configuration)
    {
        Configuration = configuration;
        db = new(Configuration.GetConnectionString(Program.Environment));
        mongoDb = new(string.Empty);
    }

    [NonAction]
    private protected async Task<byte[]> SaveImageAsync(Image image, ImageExtension extension, MemoryStream? ms)
    {
        ms ??= new();

        switch (extension)
        {
            case ImageExtension.Jpeg:
                await image.SaveAsJpegAsync(ms);
                break;

            case ImageExtension.Gif:
                await image.SaveAsGifAsync(ms);
                break;

            case ImageExtension.Bmp:
                await image.SaveAsBmpAsync(ms);
                break;

            case ImageExtension.Tga:
                await image.SaveAsTgaAsync(ms);
                break;
        }

        return await Task.Run(() => ms.ToArray());
    }
}

