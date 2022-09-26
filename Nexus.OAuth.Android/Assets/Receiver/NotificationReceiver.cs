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
using AndroidX.Work;
using Nexus.OAuth.Android.Assets.Adapters;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Models;
using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using Nexus.OAuth.Android.Assets.Api.Models.Result;
using Nexus.OAuth.Android.Assets.Fragments;
using Nexus.OAuth.Android.Assets.Models;
using Nexus.OAuth.Android.Assets.Services;
using Nexus.OAuth.Android.Base;
using System;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Threading.Tasks;
using Context = Android.Content.Context;

namespace Nexus.OAuth.Android.Assets.Receivers
{
    [BroadcastReceiver(Name = "com.nexus.oauth.AutoStartNotificationService", Exported = true)]
    [IntentFilter(new string[] {
        Intent.ActionBootCompleted,
        Intent.ActionPackagesSuspended,
        Intent.ActionUserUnlocked})]
    public class NotificationsReceiver : BroadcastReceiver
    {
        private const string TAG = NotificationsService.TAG;
        public override void OnReceive(Context ctx, Intent arg1)
        {
            Log.Info(TAG, "Receiver called.");

            WorkManager workManager = WorkManager.GetInstance(ctx);
            OneTimeWorkRequest startServiceRequest = new OneTimeWorkRequest.Builder(typeof(NotificationsWorker))
                .Build();
            workManager.Enqueue(startServiceRequest);

            ResultCode = Result.Ok;
        }
    }
}