using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.TextField;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Nexus.OAuth.Android.Assets.Fragments
{
    internal class SecondStepLoginFragment : Fragment
    {
        public const string TAG = "SecondFirstStepFragment";
        private TextInputLayout inputPassword;
        private TextInputEditText edtPassword;
        private Button btnNext;
        private TextView txtUser;
        private AppCompatCheckBox cbShowPassword;
        public FirstStepResult FirstStep { get; private set; }
        public string User { get; private set; }
        public SecondStepLoginFragment(string user, FirstStepResult firstStep)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            FirstStep = firstStep ?? throw new ArgumentNullException(nameof(firstStep));
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_login_second_step, container, false);

            inputPassword = view.FindViewById<TextInputLayout>(Resource.Id.iptPassword);
            edtPassword = view.FindViewById<TextInputEditText>(Resource.Id.edtPassword);
            btnNext = view.FindViewById<Button>(Resource.Id.btnNext);
            txtUser = view.FindViewById<TextView>(Resource.Id.txtUser);
            cbShowPassword = view.FindViewById<AppCompatCheckBox>(Resource.Id.cbShowPassword);

            cbShowPassword.CheckedChange += cbShowChanged;
            txtUser.Text = User;
            return view;
        }

        private void cbShowChanged(object sender, CompoundButton.CheckedChangeEventArgs args)
            => edtPassword.TransformationMethod = args.IsChecked ?
            (ITransformationMethod)HideReturnsTransformationMethod.Instance : 
            PasswordTransformationMethod.Instance;
    }
}