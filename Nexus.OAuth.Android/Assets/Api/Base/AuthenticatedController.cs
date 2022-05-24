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

        public override HttpRequestMessage BaseRequest
        {
            get
            {
                HttpRequestMessage request = base.BaseRequest;
                request.Headers.Add(AuthorizationHeader, Authentication.ToHeader());
                return request;
            }
        }
    }
}