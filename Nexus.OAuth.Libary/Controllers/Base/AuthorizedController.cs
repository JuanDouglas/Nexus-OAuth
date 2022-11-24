using Nexus.OAuth.Libary.Models;
using System.Net.Http.Headers;

namespace Nexus.OAuth.Libary.Controllers.Base;
internal class AuthorizedController : Controller
{
    public TokenType TokenType { get; set; }
    public string Authorization { get; set; }
    public AuthorizedController(string clientKey, string authorization, TokenType tokenType)
        : base(clientKey)
    {
        Authorization = authorization;
        TokenType = tokenType;
    }

    protected internal override HttpRequestMessage defaultRequest
    {
        get
        {
            var request = base.defaultRequest;
            request.Headers.Authorization = new AuthenticationHeaderValue(Enum.GetName(TokenType) ?? "Barear", Authorization);

            return request;
        }
    }
}