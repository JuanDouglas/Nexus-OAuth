using Android.Content;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android.Assets.Api
{
    internal class AccountController : AuthenticatedController
    {
        public override string ControllerHost => $"{Host}/Account";
        public AccountController(Context context, Authentication authentication) : base(context, authentication)
        {
        }

        public async Task<AccountResult> MyAccountAsync()
        {
            string url = $"{ControllerHost}/MyAccount";
            var request = BaseRequest;
            request.RequestUri = new Uri(url);
            HttpResponseMessage response = await SendAsync(request);

            return await CastJsonResponse<AccountResult>(response);
        }
    }
}