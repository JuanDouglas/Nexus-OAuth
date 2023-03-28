using Nexus.OAuth.Libary.Models;
using Nexus.OAuth.Libary.Models.Enums;
using Nexus.OAuth.Libary.Test.Base;
using NUnit.Framework;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace Nexus.OAuth.Libary.Test
{
    public class Authorization : BaseTest
    {
        Application App;
        HttpListener server;
        private string host = "https://localhost:44336/";
        private AccessToken access;

        public Authorization()
        {
            App = new Application(CLIENT_ID, CLIENT_SECRET);
            server = new HttpListener();
            server.Prefixes.Add(host);
            server.Start();
        }

        [SetUp]
        public async Task Setup()
        {
            Process.Start(new ProcessStartInfo(App.GenerateAuthorizeUrl(new Scope[] { Scope.Account }).ToString())
            {
                UseShellExecute = true,
                Verb = "open",
            });

            while (access == null)
            {
                HttpListenerContext ctx = server.GetContext();
                HttpListenerRequest resquet = ctx.Request;
                using HttpListenerResponse resp = ctx.Response;

                var query = HttpUtility.ParseQueryString(resquet.Url?.Query ?? "");
                string? code = query["code"] ?? string.Empty;

                if (string.IsNullOrEmpty(code))
                {
                    resp.StatusCode = (int)HttpStatusCode.BadRequest;
                    resp.StatusDescription = "Request is bad";
                }

                resp.StatusCode = (int)HttpStatusCode.OK;
                resp.StatusDescription = "Status OK";

                access = await App.GetAccessTokenAsync(code, TokenType.Barear);
            }
        }

        [Test]
        public async Task GetAccountAsync()
        {
            Account acc = await App.GetAccountAsync(access);

            byte[] img = await acc.DownloadImageAsync();
        }
    }
}