using Android.App;
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
    public abstract class AuthenticatedActivity : AppCompatActivity
    {
        public bool Authenticated { get; set; }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CheckLoginAsync(GetType())
             .Wait();
        }
        public async Task CheckLoginAsync(Type type)
        {
            try
            {
                _ = await Authentication.GetAsync(this);
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