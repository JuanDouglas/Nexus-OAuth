using Android.Content;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Assets.Api.Exceptions;
using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Net;
using System.Net.Http;
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
            string url = $"{ControllerHost}/FirstStep?user={EncodeString(user)}";
            HttpRequestMessage request = BaseRequest;
            request.RequestUri = new Uri(url);

            HttpResponseMessage response = await SendAsync(request);

            if (response.StatusCode == HttpStatusCode.NotFound ||
                response.StatusCode == HttpStatusCode.BadRequest)
                throw new UserNotFoundException();

            return await CastJsonResponse<FirstStepResult>(response);
        }
        public async Task<AuthenticationResult> SecondStepAsync(FirstStepResult result, string password, TokenType type = TokenType.Barear)
        {
            string url = $"{ControllerHost}/SecondStep?pwd={EncodeString(password)}" +
                $"&token={result.Token}" +
                $"&fs_id={result.Id}" +
                $"&tokenType={Enum.GetName(typeof(TokenType), type)}";

            HttpRequestMessage request = BaseRequest;
            request.RequestUri = new Uri(url);

            HttpResponseMessage response = await SendAsync(request);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException();

            return await CastJsonResponse<AuthenticationResult>(response);
        }
    }
}