using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Service.Notification;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Nexus.OAuth.Android.Assets.Adapters;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using Nexus.OAuth.Android.Assets.Fragments;
using Nexus.OAuth.Android.Assets.Models;
using Nexus.OAuth.Android.Base;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Nexus.OAuth.Android.Receiver
{
    [BroadcastReceiver(Name = "com.nexus.oauth.AutoStartNotificationService", Exported = true)]
    [IntentFilter(new string[] { Intent.ActionBootCompleted })]
    public class AutoStartNotificationService : BroadcastReceiver
    {
        public override void OnReceive(Context ctx, Intent arg1)
        {
            Intent intent = new Intent(ctx, typeof(NotificationsService));

            ctx.StartService(intent);

            Log.Debug("NotificationService", "receive notification");
        }
    }
}