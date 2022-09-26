using Android.App;
using Android.Content;
using Android.Util;
using AndroidX.Work;
using Nexus.OAuth.Android.Assets.Services;
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