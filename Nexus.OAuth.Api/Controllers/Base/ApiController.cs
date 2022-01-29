using Microsoft.AspNetCore.Cors;
using Nexus.OAuth.Domain.Authentication;
using Nexus.OAuth.Domain.Authentication.Exceptions;

namespace Nexus.OAuth.Api.Controllers.Base;

/// <summary>
/// Base Application Controller
/// </summary>
[EnableCors]
[RequireHttps]
[ApiController]
[Route("api/[controller]")]
[RequireAuthentication(RequireAccountValidation = true, ShowView = true)]
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
    protected static internal readonly OAuthContext db = new();

    /// <summary>
    /// Request Client Account 
    /// </summary>
    protected internal Account? ClientAccount
    {
        get
        {
            try
            {
                (TokenType tokenType, string token, _, _) = AuthenticationHelper.GetAuthorization(HttpContext);

                Task<Account?> accountTask = AuthenticationHelper.GetAccountAsync(tokenType, token);
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
    public ApiController(IConfiguration configuration) : base()
    {
        Configuration = configuration;
    }
}

