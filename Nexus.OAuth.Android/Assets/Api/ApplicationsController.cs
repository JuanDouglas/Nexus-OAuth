using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Api.Exceptions;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android.Assets.Api
{
    internal class ApplicationsController : AuthenticatedController
    {
        public override string ControllerHost => $"{DefaultURL}/Applications";

        public ApplicationsController(Authentication auth, Context context) : base(context, auth)
        {
        }

        public async Task<ApplicationResult> GetByClientIdAsync(string clientId)
        {
            string url = $"{ControllerHost}/ByClientId?" +
                $"client_id={EncodeString(clientId)}";
            var request = BaseRequest;
            request.RequestUri = new Uri(url);
            HttpResponseMessage response = await SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound)
                throw new ApplicationNotFoundException();

            return await CastJsonResponse<ApplicationResult>(response);
        }
    }
}