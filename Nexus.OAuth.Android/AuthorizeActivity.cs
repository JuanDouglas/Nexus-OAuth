using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Exceptions;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using Nexus.OAuth.Android.Assets.Fragments;
using Nexus.OAuth.Android.Base;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Nexus.OAuth.Android
{
    [Activity(Name = "com.nexus.oauth.AuthorizeActivity", Label = "@string/app_name", Theme = "@style/AppTheme.Translucent", Exported = true, MainLauncher = IsDebug)]
    [IntentFilter(new string[] { Intent.ActionSend, Intent.ActionView }, Categories = new string[] { Intent.CategoryDefault })]
    public class AuthorizeActivity : AuthenticatedActivity
    {
        private const int animDuration = 350;
        public const string ClientIdkey = "Client-Id";
        ViewGroup ltBackground;

        internal ApplicationResult OAuthApplication { get; set; }
        internal AuthorizeFragment AuthorizeFragment { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_authorize);
            if (!Authenticated)
                return;

            ltBackground = FindViewById<ViewGroup>(Resource.Id.ltBackground);

            string clientId = Intent?.GetStringExtra(ClientIdkey) ??
#if DEBUG
               "8y8gr0a51291o2q68k7b3s4kqxjc8052";
#else
               throw new ArgumentNullException(nameof(ClientIdkey));         
#endif

            LoadingTaskFragment taskFragment = new LoadingTaskFragment(new Task(()
                => LoadApplciationAsync(clientId).Wait()));

            taskFragment.Show(SupportFragmentManager, LoadingTaskFragment.TAG);
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
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async Task LoadApplciationAsync(string clientId)
        {
            if (!Authenticated)
                return;

            try
            {
                using ApplicationsController appController = new ApplicationsController(Authentication, this);
                OAuthApplication = await appController.GetByClientIdAsync(clientId);
                _ = await OAuthApplication.Logo.DownloadAsync(this);

                AuthorizeFragment = new AuthorizeFragment(OAuthApplication);

                SupportFragmentManager.BeginTransaction()
                    .SetCustomAnimations(Resource.Animation.abc_fade_in, 0)
                    .Add(Resource.Id.fgDialog, AuthorizeFragment, AuthorizeFragment.TAG)
                    .Commit();
            }
            catch (ApplicationNotFoundException)
            {
                RunOnUiThread(() =>
                {
                    Toast toast = Toast.MakeText(this, Resource.String.text_application_not_found, ToastLength.Long);
                    toast.Show();
                    Finish();
                });
            }
        }
    }
}