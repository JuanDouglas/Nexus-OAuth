using Android.Content;
using Android.OS;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using Google.Android.Material.TextField;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Exceptions;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Threading;
using System.Threading.Tasks;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Nexus.OAuth.Android.Assets.Fragments
{
    internal class FirstStepLoginFragment : Fragment
    {
        public const string TAG = "LoginFirstStepFragment";
        public event EventHandler<FirstStepSuccessEventArgs> FirstStepSuccess;
        private TextInputLayout inputUser;
        private Button btnNext;
        private AuthenticationController authController;
        Task checkLogin;
        public FirstStepLoginFragment()
        {
            FirstStepSuccess += (object sender, FirstStepSuccessEventArgs args) => { };
        }

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
        private void LoginClick(object sender, EventArgs args)
        {
            var vibrator = (VibratorManager)Context.GetSystemService(Context.VibratorManagerService);
            string user = inputUser.EditText.Text;

            if (string.IsNullOrEmpty(user))
                inputUser.Error = Resources.GetString(Resource.String.error_null);

            if (inputUser.ErrorEnabled)
            {
                vibrator.Vibrate(CombinedVibration.CreateParallel(VibrationEffect.CreatePredefined(VibrationEffect.EffectDoubleClick)));
                return;
            }

            checkLogin = new Task(() => CheckLoginAsync(user).Wait());

            LoadingTaskFragment fragment = new LoadingTaskFragment(checkLogin);
            fragment.Show(ChildFragmentManager, LoadingTaskFragment.TAG);
        }

        private async Task CheckLoginAsync(string user)
        {
            try
            {
                FirstStepResult firstStep = await authController.FirstStepAsync(user);
                var args = new FirstStepSuccessEventArgs(firstStep, user);
                FirstStepSuccess.Invoke(this, args);

                while (!args.SecondStepFragment.IsVisible)
                    Thread.Sleep(10);
            }
            catch (UserNotFoundException)
            {
                Activity.RunOnUiThread(()
                    => inputUser.Error = Resources.GetString(Resource.String.error_invalid_user));
            }
            catch (Exception ex)
            {
                Log.Debug($"Exception: {ex.Message}", ex.StackTrace);
            }
        }

        private void RemoveError(object sender, TextChangedEventArgs args)
            => inputUser.ErrorEnabled = false;

        public class FirstStepSuccessEventArgs : EventArgs
        {
            public FirstStepResult Result { get; set; }
            public SecondStepLoginFragment SecondStepFragment { get; set; }
            public string User { get; set; }

            public FirstStepSuccessEventArgs(FirstStepResult result, string user)
            {
                Result = result ?? throw new ArgumentNullException(nameof(result));
                User = user ?? throw new ArgumentNullException(nameof(user));
                SecondStepFragment = new SecondStepLoginFragment(user, result);
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            authController.Dispose();
        }
    }
}