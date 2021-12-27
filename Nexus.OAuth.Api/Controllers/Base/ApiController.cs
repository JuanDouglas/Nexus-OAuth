using Microsoft.AspNetCore.Cors;
using Nexus.OAuth.Domain.Authentication;

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
{ /// <summary>
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
            (TokenType tokenType, string token, _, _) = AuthenticationHelper.GetAuthorization(HttpContext);

            Task<Account?> accountTask = AuthenticationHelper.GetAccountAsync(tokenType, token);
            accountTask.Wait();

            return accountTask.Result;
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


    ~ApiController()
    {
        db.Dispose();
    }
}

