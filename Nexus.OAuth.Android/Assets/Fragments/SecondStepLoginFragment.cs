using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.TextField;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Nexus.OAuth.Android.Assets.Fragments
{
    public class SecondStepLoginFragment : Fragment
    {
        public const string TAG = "SecondFirstStepFragment";
        private TextInputLayout inputPassword;
        private Button btnNext;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_login_first_step, container, false);

            inputPassword = view.FindViewById<TextInputLayout>(Resource.Id.iptUser);
            btnNext = view.FindViewById<Button>(Resource.Id.btnNext);

            return view;
        }

    }
}