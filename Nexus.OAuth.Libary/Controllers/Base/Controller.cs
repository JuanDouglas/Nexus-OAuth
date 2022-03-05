namespace Nexus.OAuth.Libary.Controllers.Base
{
    internal class Controller : BaseController
    {
        protected internal const string clientKeyHeader = "Client-Key";
        public string ClientKey { get; set; }
        public Controller(string clientKey)
        {
            ClientKey = clientKey;
        }

        protected internal override HttpClient httpClient
        {
            get
            {
                HttpClient client = base.httpClient;
                client.DefaultRequestHeaders.Add(clientKeyHeader, ClientKey);

                return client;
            }
        }
    }
}
