using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
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