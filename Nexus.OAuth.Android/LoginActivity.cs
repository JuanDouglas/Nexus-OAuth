using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using AndroidX.AppCompat.App;
using Nexus.OAuth.Android.Assets.Fragments;
using System;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Nexus.OAuth.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", WindowSoftInputMode = SoftInput.AdjustPan | SoftInput.AdjustResize)]
    public class LoginActivity : AppCompatActivity
    {
        FirstStepLoginFragment fgFirstStep;
        SecondStepLoginFragment fgSecondStep;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            fgFirstStep = new FirstStepLoginFragment();

            SupportFragmentManager
                .BeginTransaction()
                .Add(Resource.Id.fgDialog, fgFirstStep, FirstStepLoginFragment.TAG)
                .Commit();
        }

        private void NextStep()
        {
            fgSecondStep = new SecondStepLoginFragment();

            SupportFragmentManager.BeginTransaction()
                .SetCustomAnimations(Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out)
                .Add(Resource.Id.fgDialog, fgFirstStep, FirstStepLoginFragment.TAG)
                .Commit();
        }
    }
}