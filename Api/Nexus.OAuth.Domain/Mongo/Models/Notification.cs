using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nexus.OAuth.Domain.Notifications.Models.Enums;

namespace Nexus.OAuth.Dal.Models;
public class Notification
{
    public struct Channels
    {
        public const string Default = "Defaults";
        public const string Security = "Security";
    }

    public struct Categories
    {
        public const string LoginSuccess = "New Login Success";
        public const string TryLogin = "Try Login";
    }

    public struct Activities
    {
        public const string QrCodeActivity = "Nexus.OAuth.Android.QrCodeActivity";
    }

    [BsonId]
    public ObjectId Id { get; private set; }
    public int UserId { get; set; }
    public NotificationStatus Status { get; set; }
    public DateTime Date { get; set; }

    public string? Activity { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Channel { get; set; }
    public string Category { get; set; }
    public Notification()
    {
    }

    public Notification(int userId, string title, string description, string channel, string category)
    {
        UserId = userId;
        Title = title;
        Description = description;
        Channel = channel;
        Category = category;
        Date = DateTime.UtcNow;
        Status = NotificationStatus.UnSended;
    }
}