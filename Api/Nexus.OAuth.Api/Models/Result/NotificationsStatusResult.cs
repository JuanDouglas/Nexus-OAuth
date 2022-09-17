using Nexus.OAuth.Dal;

namespace Nexus.OAuth.Api.Models.Result
{
    public class NotificationsStatusResult
    {
        public DateTime Date { get; set; }
        public int Length => Notifications.Length;
        public Notification[] Notifications { get; set; }

        public NotificationsStatusResult(Notification[] notifications)
        {
            Date = DateTime.UtcNow;
            Notifications = notifications;
        }
    }
}
