using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Nexus.OAuth.Android.Assets.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class AccountActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            CheckLoginAsync(this, typeof(AccountActivity), Intent.Extras)
                .Wait();
        }
        public static async Task CheckLoginAsync(Activity act, Type type, Bundle? bundle)
        {
            try
            {
                Authentication authentication = await Authentication.GetAsync(act);
            }
            catch (UnauthorizedAccessException)
            {
                Intent intent = new Intent(act, typeof(LoginActivity));
                Bundle animBundle = ActivityOptionsCompat.MakeCustomAnimation(act, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out)
                    .ToBundle();

                intent.PutExtra(LoginActivity.AfterActivityKey, type.AssemblyQualifiedName);
                intent.PutExtra(LoginActivity.AfterBundleKey, bundle);
                ActivityCompat.StartActivity(act, intent, animBundle);
                act.Finish();
            }
        }
    }
}