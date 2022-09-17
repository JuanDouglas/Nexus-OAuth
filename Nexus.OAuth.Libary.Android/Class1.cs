using Android.Content;
using System;

namespace Nexus.OAuth.Libary.Android
{
    public class Authorization
    {
        private const string OAuthPkg = "com.nexus.oauth";
        private const string OAuthActivity = "com.nexus.oauth.AuthorizeActivity";

        public string ClientId { get; set; }

        public Authorization(string clientId)
        {
            ClientId = clientId;
        }

        public void Request(Context ctx, Type after)
        {
            Intent intent = new Intent();
            intent.SetComponent(new ComponentName(OAuthPkg, OAuthActivity));
            intent.PutExtra("Client-Id", ClientId);
            intent.PutExtra("AfterPkg", ctx.PackageName);
            intent.PutExtra("AfterIntent", after.FullName);
            ctx.StartActivity(intent);
        }
    }
}
