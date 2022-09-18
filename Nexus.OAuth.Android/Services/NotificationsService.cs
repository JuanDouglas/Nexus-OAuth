using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Service.Notification;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidX.Core.App;
using AndroidX.RecyclerView.Widget;
using Javax.Net.Ssl;
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
using System.Linq;
using System.Threading.Tasks;
using WebSocketSharp;
using Notification = Nexus.OAuth.Android.Assets.Api.Models.Result.Notification;

namespace Nexus.OAuth.Android
{
    [Service(Name = "com.nexus.oauth.NotificationsService", Enabled = true)]
    public class NotificationsService : Service
    {
        public static bool Started { get; set; }
        internal static NotificationsController? notificationsController;
        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            Started = false;
            base.OnDestroy();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            Log.Info("NotificationService", "Started successful ");
            Started = true;

            var authTask = Authentication.GetAsync(this);
            authTask.Wait();

            notificationsController ??= new NotificationsController(this, authTask.Result);

            notificationsController.NewNotification += NewMessage;
            notificationsController.Connect();

            return base.OnStartCommand(intent, flags, startId);
        }

        private void NewMessage(DateTime update, Notification[] notifications)
        {
            NotificationManagerCompat manager = NotificationManagerCompat.From(this);


            foreach (var item in notifications)
            {
                var noti = new NotificationCompat.Builder(this, item.Channel)
                                    .SetSmallIcon(Resource.Drawable.nexus_crystal)
                                    .SetContentTitle(item.Title)
                                    .SetContentText(item.Description)
                                    .SetStyle(new NotificationCompat.BigTextStyle())
                                    .Build();

                if (manager.NotificationChannelsCompat.FirstOrDefault(fs => fs.Id == item.Channel) == null)
                {
                    CreateNotificationChannel(item.Channel, "Notification Channel");
                }

                // notificationId is a unique int for each notification that you must define
                manager.Notify(item.IntegerId, noti);
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