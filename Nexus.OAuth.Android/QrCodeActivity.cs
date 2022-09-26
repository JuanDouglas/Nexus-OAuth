using Android.App;
using Android.Content;
using Android.OS;
using Nexus.OAuth.Android.Assets.Api.Base;
using Nexus.OAuth.Android.Base;

namespace Nexus.OAuth.Android
{
    [Activity(Name = "com.nexus.oauth.QrCodeActivity", Label = "@string/app_name", Theme = "@style/AppTheme.Translucent", Exported = true, MainLauncher = IsDebug)]
    [IntentFilter(new string[] { Intent.ActionSend, Intent.ActionView }, Categories = new string[] { Intent.CategoryDefault, Intent.CategoryBrowsable }, DataScheme = "https", DataHost = BaseApiController.ApiHost, DataPathPrefix = "/api/Authentications/QrCode/Authorize")]
    public class QrCodeActivity : AuthenticatedActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_qrcode);
            // Create your application here
        }
    }
}