using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Nexus.OAuth.Android.Assets.Fragments;
using System;
using static Nexus.OAuth.Android.Assets.Fragments.FirstStepLoginFragment;
using static Nexus.OAuth.Android.Assets.Fragments.SecondStepLoginFragment;

namespace Nexus.OAuth.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.AdjustResize)]
    public class LoginActivity : AppCompatActivity
    {
        public const string AfterActivityKey = "After";
        public const string AfterBundleKey = "After-Bundle";
        public Type AfterActivityType { get; set; }
        public Bundle? AfterBundle { get; set; }

        FirstStepLoginFragment fgFirstStep;
        SecondStepLoginFragment fgSecondStep;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_login);

            string qlfName = Intent.Extras.GetString(AfterActivityKey);
            AfterActivityType = Type.GetType(qlfName);
            AfterBundle = Intent.Extras.GetBundle(AfterBundleKey);

            fgFirstStep = new FirstStepLoginFragment();
            fgFirstStep.FirstStepSuccess += NextStep;

            SupportFragmentManager
                    .BeginTransaction()
                    .Add(Resource.Id.fgDialog, fgFirstStep, FirstStepLoginFragment.TAG)
                    .Commit();
        }
        private void Authenticated(object sender, AuthenticationSuccessEventArgs args)
        {
            Intent intent = new Intent(this, AfterActivityType);
            Bundle animBundle = ActivityOptionsCompat.MakeCustomAnimation(this, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out)
                .ToBundle();

            if (AfterBundle != null)
                intent.PutExtras(AfterBundle);

            ContextCompat.StartActivity(this, intent, animBundle);
            Finish();
        }
        private void NextStep(object sender, FirstStepSuccessEventArgs args)
        {
            fgSecondStep = args.SecondStepFragment;
            fgSecondStep.AuthenticationSuccess += Authenticated;

            RunOnUiThread(()
                => SupportFragmentManager.BeginTransaction()
                    .SetCustomAnimations(Resource.Animation.fragment_slide_in_right, Resource.Animation.fragment_slide_out_left)
                    .Remove(fgFirstStep)
                    .Add(Resource.Id.fgDialog, fgSecondStep, SecondStepLoginFragment.TAG)
                    .Commit());
        }
        public override void OnBackPressed()
        {
            if (fgSecondStep?.IsVisible ?? false)
            {
                SupportFragmentManager
                    .BeginTransaction()
                    .SetCustomAnimations(Resource.Animation.fragment_slide_in_left, Resource.Animation.fragment_slide_out_right)
                    .Remove(fgSecondStep)
                    .Add(Resource.Id.fgDialog, fgFirstStep, FirstStepLoginFragment.TAG)
                    .Commit();

                return;
            }

            base.OnBackPressed();
        }
    }
}