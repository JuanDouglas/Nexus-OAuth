using Android.Content;
using Android.OS;
using Android.Text;
using Android.Text.Method;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.TextField;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Threading.Tasks;
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
        private AuthenticationController authController;
        public event EventHandler<AuthenticationSuccessEventArgs> AuthenticationSuccess;
        public FirstStepResult FirstStep { get; private set; }
        public string User { get; private set; }
        protected SecondStepLoginFragment()
        {

        }
        public SecondStepLoginFragment(string user, FirstStepResult firstStep) : this()
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            FirstStep = firstStep ?? throw new ArgumentNullException(nameof(firstStep));
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_login_second_step, container, false);
            authController = new AuthenticationController(view.Context);

            inputPassword = view.FindViewById<TextInputLayout>(Resource.Id.iptPassword);
            edtPassword = view.FindViewById<TextInputEditText>(Resource.Id.edtPassword);
            btnNext = view.FindViewById<Button>(Resource.Id.btnNext);
            txtUser = view.FindViewById<TextView>(Resource.Id.txtUser);
            cbShowPassword = view.FindViewById<AppCompatCheckBox>(Resource.Id.cbShowPassword);

            cbShowPassword.CheckedChange += cbShowChanged;
            inputPassword.EditText.TextChanged += RemoveError;
            btnNext.Click += NextClick;
            txtUser.Text = User;
            return view;
        }

        private void RemoveError(object sender, TextChangedEventArgs args)
            => inputPassword.ErrorEnabled = false;

        private void cbShowChanged(object sender, CompoundButton.CheckedChangeEventArgs args)
            => edtPassword.TransformationMethod = args.IsChecked ?
            (ITransformationMethod)HideReturnsTransformationMethod.Instance :
            PasswordTransformationMethod.Instance;

        private void NextClick(object sender, EventArgs args)
        {
            var vibrator = (VibratorManager)Context.GetSystemService(Context.VibratorManagerService);
            string password = inputPassword.EditText.Text;

            if (cbShowPassword.Checked)
                cbShowPassword.Checked = false;

            if (string.IsNullOrEmpty(password))
                inputPassword.Error = Resources.GetString(Resource.String.error_null);

            if (inputPassword.ErrorEnabled)
            {
                vibrator.Vibrate(CombinedVibration.CreateParallel(VibrationEffect.CreatePredefined(VibrationEffect.EffectDoubleClick)));
                return;
            }

            LoadingTaskFragment taskFragment = new LoadingTaskFragment(new Task(() => CheckLoginAsync(password).Wait()));
            taskFragment.Show(ChildFragmentManager, LoadingTaskFragment.TAG);
        }

        private async Task CheckLoginAsync(string pass)
        {
            try
            {
                AuthenticationResult result = await authController.SecondStepAsync(FirstStep, inputPassword.EditText.Text);
                Authentication authentication = new Authentication(result, FirstStep);
                await authentication.SaveAsync(Activity);
                AuthenticationSuccess.Invoke(this, new AuthenticationSuccessEventArgs(authentication));
            }
            catch (UnauthorizedAccessException)
            {
                Activity.RunOnUiThread(()
                    => inputPassword.Error = Resources.GetString(Resource.String.error_invalid_password));
            }
        }

        public class AuthenticationSuccessEventArgs : EventArgs
        {
            public Authentication Authentication { get; set; }

            public AuthenticationSuccessEventArgs(Authentication authentication)
            {
                Authentication = authentication ?? throw new ArgumentNullException(nameof(authentication));
            }
        }
    }
}