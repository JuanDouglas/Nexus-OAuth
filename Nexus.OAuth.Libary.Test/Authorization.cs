using Nexus.OAuth.Libary.Exceptions;
using Nexus.OAuth.Libary.Models;
using Nexus.OAuth.Libary.Test.Base;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Nexus.OAuth.Libary.Test
{
    public class Authorization : BaseTest
    {
        Application application;
        Client client;

        [SetUp]
        public void Setup()
        {
            application = new(CLIENT_ID, CLIENT_SECRET);
            client = new("", TokenType.Barear, "", "");
        }

        [Test]
        public async Task LoginSuccess()
        {
            client = await Client.LoginAsync(User, Password);
            Assert.Pass();
        }

        [Test]
        public async Task LoginFailed()
        {
            try
            {
                Client client = await Client.LoginAsync(User, Password + 'a');
                Assert.Fail();
            }
            catch (AuthenticationException)
            {
                Assert.Pass();
                throw;
            }
        }

        [Test]
        public async Task AuthorizeSuccess()
        {
            await client.AuthorizeAsync(CLIENT_ID, "user");
        }
    }
}