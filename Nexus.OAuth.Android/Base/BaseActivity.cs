using Android.App;
using Android.OS;
using AndroidX.AppCompat.App;

namespace Nexus.OAuth.Android.Base
{
    [Activity(Label = "BaseActivity")]
    public abstract class BaseActivity : AppCompatActivity
    {
        public const bool IsDebug =
#if DEBUG
            true;
#else   
            false;
#endif
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your application here
        }
    }
}