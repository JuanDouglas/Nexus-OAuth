using Nexus.OAuth.Android.Assets.Api.Models.Enums;
using System;

namespace Nexus.OAuth.Android.Assets.Api.Models.Result
{
    internal class NotificationsStatusResult
    {
        public DateTime Date { get; set; }
        public int Length => Notifications.Length;
        public Notification[] Notifications { get; set; }
    }

    internal class Notification
    {
        public string Id { get; set; }
        public int IntegerId { get; private set; }
        public int UserId { get; set; }
        public NotificationStatus Status { get; set; }
        public DateTime Date { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public string Channel { get; set; }
        public string Category { get; set; }

        static Random? random;
        public Notification()
        {
            random ??= new Random();
            IntegerId = random.Next();
        }
    }
}