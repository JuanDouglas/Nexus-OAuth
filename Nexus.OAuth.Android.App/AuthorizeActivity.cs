using Android.App;
using Android.Content;
using Android.OS;

namespace Nexus.OAuth.Android.App
{
    [Activity(Label = "AuthorizeActivity")]
    [IntentFilter(new[] { Intent.ActionView },
         Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
              DataScheme = "https",
              DataHost = "nexus-oauth.azurewebsites.net",
              DataPathPrefix = "/authentication/authorize",
              AutoVerify = true)]
    public class AuthorizeActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
        }
    }
}