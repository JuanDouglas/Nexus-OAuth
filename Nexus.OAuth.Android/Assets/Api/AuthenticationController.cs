using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Api.Exceptions;
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
    internal class AuthenticationController : BaseApiController
    {
        public override string ControllerHost => $"{Host}/Authentications";
        public AuthenticationController(Context context) : base(context)
        {
        }
        public async Task<FirstStepResult> FirstStepAsync(string user)
        {
            string url = $"{ControllerHost}/FirstStep?user={user}";
            HttpRequestMessage request = BaseRequest;
            request.RequestUri = new Uri(url);

            var response = await HttpClient.SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound ||
                response.StatusCode == HttpStatusCode.BadRequest)
                throw new UserNotFoundException();

            return await CastJsonResponse<FirstStepResult>(response);
        }
    }
}