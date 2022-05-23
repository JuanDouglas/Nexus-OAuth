using Microsoft.AspNetCore.Cors;
using Nexus.OAuth.Domain.Authentication;
using Nexus.OAuth.Domain.Authentication.Exceptions;
using Nexus.OAuth.Domain.Messages;
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
    private protected readonly OAuthContext db;

    /// <summary>
    /// Request Client Account 
    /// </summary>
    private protected Account? ClientAccount
    {
        get
        {
            try
            {
                (TokenType tokenType, string[] tokens, _) = AuthenticationHelper.GetAuthorization(HttpContext);

                Task<Account?> accountTask = Program.AuthenticationHelper.GetAccountAsync(tokenType, tokens[0], db);
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

    /// <summary>
    /// 
    /// </summary>
    public IConfiguration Configuration { get => _configuration; }
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Application Email message sender class object instance
    /// </summary>
    private protected EmailMessageSender EmailSender { get => _emailSender; }
    private readonly EmailMessageSender _emailSender;

    /// <summary>
    /// 
    /// </summary>
    private protected string WebHost { get => _webHost; }
    private readonly string _webHost;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    public ApiController(IConfiguration configuration)
    {
        _configuration = configuration;
        _emailSender = new EmailMessageSender(Configuration);
        _webHost = Configuration.GetSection("WebHost").Value;
        db = new(Configuration.GetConnectionString("SqlServer"));
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

