namespace Nexus.OAuth.Libary.Controllers.Base;

internal class Controller : BaseController
{
    protected internal const string clientKeyHeader = "Client-Key";
    public string ClientKey { get; set; }

    protected internal override HttpRequestMessage defaultRequest
    {
        get
        {
            var request = base.defaultRequest;
            request.Headers.Add(clientKeyHeader, ClientKey);

            return request;
        }
    }
    public Controller(string clientKey)
    {
        ClientKey = clientKey;
    }
    public Controller(string clientKey, string productName, string? productVersion)
    {
        ClientKey = clientKey;
        ProductName = productName;
        ProductVersion = productVersion;
    }
}
