using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Nexus.OAuth.Android.Assets.Api.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Nexus.OAuth.Android
{
    [Activity(Name = "com.nexus.oauth.AuthorizeActivity", Label = "@string/app_name", Theme = "@style/AppTheme.Translucent", Exported = true)]
    [IntentFilter(new string[] { Intent.ActionSend, Intent.ActionView }, Categories = new string[] { Intent.CategoryDefault })]
    public class AuthorizeActivity : AppCompatActivity
    {
        private const int animDuration = 350;
        ViewGroup ltBackground;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_authorize);
            AlphaAnimation anim = new AlphaAnimation(0f, 1f)
            {
                Duration = animDuration,
                Interpolator = new AccelerateDecelerateInterpolator(),
                FillAfter = true,
                FillBefore = true
            };

            AccountActivity
                .CheckLoginAsync(this, typeof(AuthorizeActivity), Intent.Extras)
                .Wait();

            ltBackground = FindViewById<ViewGroup>(Resource.Id.ltBackground);
            ltBackground.Click += FinishAuthentication;
            ltBackground.StartAnimation(anim);
        }

        private void FinishAuthentication(object sender, EventArgs args)
        {
            AlphaAnimation anim = new AlphaAnimation(1f, 0f)
            {
                Duration = animDuration,
                Interpolator = new AccelerateInterpolator()
            };
            anim.AnimationEnd += (object sender, Animation.AnimationEndEventArgs args)
                => Process.KillProcess(Process.MyPid());

            ltBackground
                 .StartAnimation(anim);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //protected override void OnPause()
        //{
        //    base.OnPause();
        //    FinishAuthentication(null, null);
        //}

        //public override void OnBackPressed()
        //{
        //    base.OnBackPressed();
        //    FinishAuthentication(null, null);
        //}
    }
}