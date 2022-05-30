using Android.Content;
using Nexus.OAuth.Android.Assets.Api.Models;
using System;
using System.Net.Http;

namespace Nexus.OAuth.Android.Assets.Api.Base
{
    internal abstract class AuthenticatedController : BaseApiController
    {
        private const string AuthorizationHeader = "Authorization";
        public Authentication Authentication { get; set; }
        protected AuthenticatedController(Context context, Authentication authentication) : base(context)
        {
            Authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
        }

        public override HttpRequestMessage BaseRequest => AddAuthentication(base.BaseRequest, Authentication);

        public static HttpRequestMessage AddAuthentication(HttpRequestMessage request, Authentication authentication)
        {
            request.Headers.Add(AuthorizationHeader, authentication.ToHeader());
            return request;
        }
    }
}