using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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