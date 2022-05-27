using Android.App;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Nexus.OAuth.Android.Assets.Adapters;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using Nexus.OAuth.Android.Assets.Fragments;
using Nexus.OAuth.Android.Assets.Models;
using Nexus.OAuth.Android.Base;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class AccountActivity : AuthenticatedActivity
    {
        private View liquid;
        private TextView txtUser;
        private TextView txtEmail;
        private Button btnLogout;
        private RecyclerView rcvInfos;
        private AccountController accountController;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_account);

            txtUser = FindViewById<TextView>(Resource.Id.txtUser);
            txtEmail = FindViewById<TextView>(Resource.Id.txtEmail);
            btnLogout = FindViewById<Button>(Resource.Id.btnLogout);
            rcvInfos = FindViewById<RecyclerView>(Resource.Id.rcvInfos);
            liquid = FindViewById(Resource.Id.imgLiquid);

            Task ts = new Task(() => LoadAsync().Wait());
            ts.Start();
        }

        private void LogoutClick(object sender, EventArgs args)
        {
            LoadingTaskFragment taskFragment = new LoadingTaskFragment(new Task(async () => await Logout()));
            taskFragment.Show(SupportFragmentManager, LoadingTaskFragment.TAG);
        }

        private async Task Logout()
        {
            await Authentication.LogoutAsync();

            await CheckLoginAsync(typeof(AccountActivity));
        }

        private async Task LoadAsync()
        {
            if (!Authenticated)
                return;

            Authentication auth = await Authentication.GetAsync(this);
            accountController = new AccountController(this, auth);

            AccountResult account = await accountController.MyAccountAsync();
            string[] splName = account.Name.Split(' ');
            RegionInfo region = new RegionInfo(CultureInfo.GetCultureInfo(account.Culture).LCID);
            Info[] infos = new Info[]
            {
                new Info(Resources.GetString(Resource.String.info_name), account.Name),
                new Info(Resources.GetString(Resource.String.info_phone), account.Phone),
                new Info(Resources.GetString(Resource.String.info_email), account.Email),
                new Info(Resources.GetString(Resource.String.info_date_of_birth), account.DateOfBirth.ToLocalTime().ToString("dd/MM/yyyy")),
                new Info(Resources.GetString(Resource.String.info_culture), region.NativeName),
                new Info(Resources.GetString(Resource.String.info_created), account.Created.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), true),
            };

            RunOnUiThread(() =>
            {
                txtUser.Text = $"{splName[0]} {splName[1]}";
                txtEmail.Text = account.Email;
                btnLogout.Click += LogoutClick;
                rcvInfos.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
                rcvInfos.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
                rcvInfos.SetAdapter(new InfosAdapter(infos));
            });

            Animation anim = AnimationUtils.LoadAnimation(this, Resource.Animation.anim_fluid_account);
            anim.AnimationEnd += (object sender, Animation.AnimationEndEventArgs args) =>
           {
               try
               {
                   liquid.Visibility = ViewStates.Gone;
                   btnLogout.Visibility = ViewStates.Visible;
               }
               catch (Exception ex)
               {
               }
           };

            liquid.StartAnimation(anim);
        }
    }
}