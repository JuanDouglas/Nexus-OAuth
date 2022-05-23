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
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Nexus.OAuth.Android.Assets.Fragments
{
    public class FirstStepLoginFragment : Fragment
    {
        public const string TAG = "LoginFirstStepFragment";
        private TextInputLayout inputUser;
        private Button btnNext;
        private AuthenticationController authController;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_login_first_step, container, false);

            authController = new AuthenticationController(view.Context);
            inputUser = view.FindViewById<TextInputLayout>(Resource.Id.iptUser);
            btnNext = view.FindViewById<Button>(Resource.Id.btnNext);

            btnNext.Click += LoginClick;
            inputUser
                .EditText
                .TextChanged += RemoveError;

            return view;
        }


        private async void LoginClick(object sender, EventArgs args)
        {
            var vibrator = (VibratorManager)Context.GetSystemService(Context.VibratorManagerService);

            if (string.IsNullOrEmpty(inputUser.EditText.Text))
                inputUser.Error = Resources.GetString(Resource.String.error_null_user);

            if (inputUser.ErrorEnabled)
            {
                vibrator.Vibrate(CombinedVibration.CreateParallel(VibrationEffect.CreatePredefined(VibrationEffect.EffectDoubleClick)));
                return;
            }

            try
            {
                await authController.FirstStepAsync(inputUser.EditText.Text);
            }
            catch (Exception)
            {
                inputUser.Error = Resources.GetString(Resource.String.error_invalid_user);
            }
        }

        private void RemoveError(object sender, TextChangedEventArgs args)
            => inputUser.ErrorEnabled = false;

    }
}