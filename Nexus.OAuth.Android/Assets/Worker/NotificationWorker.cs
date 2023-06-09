﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using AndroidX.Core.App;
using AndroidX.Work;
using Nexus.OAuth.Android.Assets.Api;
using Nexus.OAuth.Android.Assets.Api.Models;
using System;
using System.Linq;
using Notification = Nexus.OAuth.Android.Assets.Api.Models.Result.Notification;

namespace Nexus.OAuth.Android.Assets.Services
{
    public class NotificationsWorker : Worker
    {
        private const string TAG = NotificationsService.TAG;
        private Context ctx;
        public NotificationsWorker(Context context, WorkerParameters workerParams) : base(context, workerParams)
        {
            ctx = context;
        }

        public override Result DoWork()
        {
            Log.Info(TAG, "Service Work called for: " + Id);

            var authTask = Authentication.GetAsync(ctx);
            authTask.Wait();

            var notificationsController = new NotificationsController(ctx, authTask.Result);
            notificationsController.NewNotification += NewMessage;
            notificationsController.Error += Error;

            try
            {
                notificationsController.Connect();
            }
            catch (Exception ex)
            {
                Log.Error(TAG, $"Start service error for exception {ex}");
            }

            while (!notificationsController.Connected)
            {
            }

            return new Result.Success();
        }

        private void Error(object sender, Exception ex)
        {
        }
        private void NewMessage(DateTime update, Notification[] notifications)
        {
            NotificationManagerCompat manager = NotificationManagerCompat.From(ctx);
            var random = new Random();
            foreach (var item in notifications)
            {
                var noti = new NotificationCompat.Builder(ctx, item.Channel)
                                    .SetSmallIcon(Resource.Drawable.crystal_contour)
                                    .SetContentTitle(item.Title)
                                    .SetContentText(item.Description)
                                    .SetPriority((int)NotificationPriority.Default)
                                    .SetStyle(new NotificationCompat.BigTextStyle())
                                    .SetCategory(item.Category);

                if (!string.IsNullOrEmpty(item.Activity))
                {
                    Type type = typeof(Notification).Assembly.GetType(item.Activity);

                    if (type != null)
                    {
                        Intent intent = new Intent(ctx, type);
                        PendingIntent pdIntent = PendingIntent.GetActivity(ctx, random.Next(), intent, PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent);
                        noti.SetContentIntent(pdIntent);
                    }
                }

                if (manager.NotificationChannelsCompat.FirstOrDefault(fs => fs.Id == item.Channel) == null)
                {
                    CreateNotificationChannel(item.Channel, "Notification Channel");
                }

                // notificationId is a unique int for each notification that you must define
                manager.Notify(item.IntegerId, noti.Build());
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
                NotificationManagerCompat notificationManager = NotificationManagerCompat.From(ctx);
                var builder = new NotificationChannelCompat.Builder(id, (int)NotificationImportance.Default);

                builder.SetName(name);
                // Register the channel with the system; you can't change the importance
                // or other notification behaviors after this
                notificationManager.CreateNotificationChannel(builder.Build());
            }
        }
    }
}