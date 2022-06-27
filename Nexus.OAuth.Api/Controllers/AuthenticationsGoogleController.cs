using Google.Apis.Auth.OAuth2;
using Nexus.OAuth.Api.Controllers.Base;

namespace Nexus.OAuth.Api.Controllers;

[Route("api/Authentications/Google")]
public class AuthenticationsGoogleController : ApiController
{
    internal ClientSecrets Secrets { get; set; }
    public AuthenticationsGoogleController(IConfiguration config) : base(config)
    {
        Secrets = config.GetSection("GoogleOAuth").Get<ClientSecrets>();
    }
}