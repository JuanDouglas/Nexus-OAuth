﻿using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using Nexus.OAuth.Android.Assets.Api.Models;
using System;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android.Base
{
    [Activity(Label = "AuthenticatedController")]
    public abstract class AuthenticatedActivity : BaseActivity
    {
        public bool Authenticated { get; set; }

        private protected Authentication? Authentication => _authentication;
        private Authentication? _authentication;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckLoginAsync(GetType())
             .Wait();

            Task startService = new Task(() =>
            {
                if (!NotificationsService.Started && Authenticated)
                {
                    Intent ntsIntent = new Intent(this, typeof(NotificationsService));

                    StartService(ntsIntent);
                }
            });

            startService.Start();
        }
        public async Task CheckLoginAsync(Type type)
        {
            try
            {
                _authentication = await Authentication.GetAsync(this);
                Authenticated = true;
            }
            catch (UnauthorizedAccessException)
            {
                Intent intent = new Intent(this, typeof(LoginActivity));
                Bundle animBundle = ActivityOptionsCompat.MakeCustomAnimation(this, Resource.Animation.abc_slide_in_bottom, Resource.Animation.abc_fade_out)
                    .ToBundle();

                intent.PutExtra(LoginActivity.AfterActivityKey, type.AssemblyQualifiedName);
                intent.PutExtra(LoginActivity.AfterBundleKey, Intent.Extras);
                ActivityCompat.StartActivity(this, intent, animBundle);
                Finish();
            }
        }
    }
}