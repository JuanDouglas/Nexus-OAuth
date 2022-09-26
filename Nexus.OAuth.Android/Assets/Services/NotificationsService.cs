using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using AndroidX.Core.App;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Receivers;
using System;
using System.Linq;
using Notification = Nexus.OAuth.Android.Assets.Api.Models.Result.Notification;

namespace Nexus.OAuth.Android.Assets.Services
{

    [Service(Name = "com.nexus.oauth.NotificationsService", Enabled = true, Exported = true)]
    public class NotificationsService : Service
    {
        public const string TAG = "NotifyService";
        public static bool IsRunning { get; set; }
        internal static NotificationsController notificationsController;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            IsRunning = true;

            Intent broadcastIntent = new Intent(this, typeof(NotificationsReceiver));
            SendBroadcast(broadcastIntent);

            return StartCommandResult.Sticky;
        }
        private void NewMessage(DateTime update, Notification[] notifications)
        {
            NotificationManagerCompat manager = NotificationManagerCompat.From(this);

            foreach (var item in notifications)
            {
                var noti = new NotificationCompat.Builder(this, item.Channel)
                                    .SetSmallIcon(Resource.Drawable.crystal_contour)
                                    .SetContentTitle(item.Title)
                                    .SetContentText(item.Description)
                                    .SetPriority((int)NotificationPriority.Default)
                                    .SetStyle(new NotificationCompat.BigTextStyle())
                                    .SetCategory(item.Category)
                                    .Build();

                if (manager.NotificationChannelsCompat.FirstOrDefault(fs => fs.Id == item.Channel) == null)
                {
                    CreateNotificationChannel(item.Channel, "Notification Channel");
                }

                // notificationId is a unique int for each notification that you must define
                manager.Notify(item.IntegerId, noti);
#if DEBUG
                Log.Debug(TAG, $"New notification {item}");
#endif
            }
        }

        private void CreateNotificationChannel(string id, string name)
        {

            // Create the NotificationChannel, but only on API 26+ because
            // the NotificationChannel class is new and not in the support library
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                NotificationManagerCompat notificationManager = NotificationManagerCompat.From(this);
                var builder = new NotificationChannelCompat.Builder(id, (int)NotificationImportance.Default);

                builder.SetName(name);
                // Register the channel with the system; you can't change the importance
                // or other notification behaviors after this
                notificationManager.CreateNotificationChannel(builder.Build());
            }
        }
    }
}