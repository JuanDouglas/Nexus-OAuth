using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Core.App;
using AndroidX.RecyclerView.Widget;
using Nexus.OAuth.Android.Assets.Adapters;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using Nexus.OAuth.Android.Assets.Fragments;
using Nexus.OAuth.Android.Assets.Models;
using Nexus.OAuth.Android.Base;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class AccountActivity : AuthenticatedActivity
    {
        private TextView txtUser;
        private TextView txtEmail;
        private Button btnLogout;
        private RecyclerView rcvInfos;
        private AccountController accountController;
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_account);
            if (!Authenticated)
                return;

            txtUser = FindViewById<TextView>(Resource.Id.txtUser);
            txtEmail = FindViewById<TextView>(Resource.Id.txtEmail);
            btnLogout = FindViewById<Button>(Resource.Id.btnLogout);
            rcvInfos = FindViewById<RecyclerView>(Resource.Id.rcvInfos);

            Authentication auth = await Authentication.GetAsync(this);
            accountController = new AccountController(this, auth);

            AccountResult account = await accountController.MyAccountAsync();
            string[] splName = account.Name.Split(' ');
            Info[] infos = new Info[]
            {
                new Info(Resources.GetString(Resource.String.info_name), account.Name),
                new Info(Resources.GetString(Resource.String.info_phone), account.Phone),
                new Info(Resources.GetString(Resource.String.info_email), account.Email),
                new Info(Resources.GetString(Resource.String.info_date_of_birth), account.DateOfBirth.ToLocalTime().ToString("dd/MM/yyyy")),
                new Info(Resources.GetString(Resource.String.info_culture), new RegionInfo(CultureInfo.GetCultureInfo(account.Culture).LCID).DisplayName),
                new Info(Resources.GetString(Resource.String.info_created), account.Created.ToLocalTime().ToString("dd/MM/yyyy HH:mm:ss"), true),
            };

            txtUser.Text = $"{splName[0]} {splName[1]}";
            txtEmail.Text = account.Email;
            btnLogout.Click += LogoutClick;
            rcvInfos.AddItemDecoration(new DividerItemDecoration(this, DividerItemDecoration.Vertical));
            rcvInfos.SetLayoutManager(new LinearLayoutManager(this, LinearLayoutManager.Vertical, false));
            rcvInfos.SetAdapter(new InfosAdapter(infos));
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
    }
}