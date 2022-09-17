using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Fragment = AndroidX.Fragment.App.Fragment;
using Uri = Android.Net.Uri;

namespace Nexus.OAuth.Android.Assets.Fragments
{
    internal class AuthorizeFragment : Fragment
    {
        public const string TAG = "AuthorizeFragment";
        public ApplicationResult OAuthApplication { get; set; }

        private TextView txtName;
        private TextView txtDescription;
        private TextView txtReadMore;
        private Button btnAuthorize;
        private Button btnDecline;
        private ImageView imgLogo;

        public AuthorizeFragment(ApplicationResult app)
        {
            OAuthApplication = app;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View view = inflater.Inflate(Resource.Layout.fragment_authorize, container, false);
            imgLogo = view.FindViewById<ImageView>(Resource.Id.imgLogo);
            txtName = view.FindViewById<TextView>(Resource.Id.txtName);
            txtDescription = view.FindViewById<TextView>(Resource.Id.txtDescription);
            txtReadMore = view.FindViewById<TextView>(Resource.Id.txtReadMore);
            btnAuthorize = view.FindViewById<Button>(Resource.Id.btnAuthorize);
            btnDecline = view.FindViewById<Button>(Resource.Id.btnDecline);

            imgLogo.SetImageDrawable(OAuthApplication.Logo.LastDrawable);
            txtName.Text = OAuthApplication.Name;
            txtDescription.Text = OAuthApplication.Description;
            btnAuthorize.Click += AuthorizeClick;
            txtReadMore.Click += ReadMoreClick;
            btnDecline.Click += DeclineClick;
            return view;
        }

        private void DeclineClick(object sender, EventArgs e)
            => Activity.Finish();

        private void AuthorizeClick(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ReadMoreClick(object sender, EventArgs args)
        {
            Intent browserIntent = new Intent(Intent.ActionView, Uri.Parse(OAuthApplication.Site));
            StartActivity(browserIntent);
        }
    }
}